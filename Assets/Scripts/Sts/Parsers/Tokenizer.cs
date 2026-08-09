using System;
using System.Linq;
using System.Text;
using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    public static class Tokenizer {
        public static bool IsValidNameCharacter(char c) => c == '_' || char.IsLetterOrDigit(c);

        public static bool IsValidName(ReadOnlySpan<char> s) {
            for (int i = 0; i < s.Length; i++) {
                if (!IsValidNameCharacter(s[i])) return false;
            }

            return true;
        }

        public static LineTokens TokenizeAndAdvance(ParsingState parsingState) {
            int lineNumber = parsingState.LineNumber;
            string line = parsingState.MoveNext();

            ReadOnlySpan<char> actualLineSpan = GetActualSpanFromLine(line);
            if (actualLineSpan.IsEmpty) {
                throw new Exception("Unexpected state");
            }

            int cmdEnd = 0;
            for (int i = 0; i < actualLineSpan.Length; i++) {
                char c = actualLineSpan[i];
                if (IsValidNameCharacter(c) || c == '!') {
                    cmdEnd++;
                } else {
                    break;
                }
            }

            if (cmdEnd == 0) {
                throw new ParsingException(lineNumber, line, "Failed to parse command name");
            }

            string cmd = actualLineSpan[..cmdEnd].ToString();
            string[] args = null;

            ReadOnlySpan<char> remaining = actualLineSpan[cmdEnd..].Trim();
            if (remaining.StartsWith("(")) {
                int indexOfClose = remaining.IndexOf(')');
                if (indexOfClose < 0) {
                    throw new ParsingException(lineNumber, line, "Encounter '(' without ')'");
                }

                args = remaining[1..indexOfClose].ToString().Split(',').Select(s => s.Trim()).ToArray();
                remaining = remaining[(indexOfClose + 1)..].Trim();
            }

            if (remaining.IsEmpty) {
                return new LineTokens(lineNumber, line, cmd, args, null);
            }

            if (!remaining.StartsWith(":")) {
                throw new ParsingException(lineNumber, line, "Expected ':' before text");
            }

            string text = ReadText(remaining[1..], out bool isTextContinued);

            if (isTextContinued) {
                ReadContinuingText(parsingState, ref text);
            }

            return new LineTokens(lineNumber, line, cmd, args, text);
        }

        public static ReadOnlySpan<char> TokenizeCommandName(in ParsingState parsingState) {
            string line = parsingState.CurrentLine;
            ReadOnlySpan<char> actualSpan = GetActualSpanFromLine(line);

            int cmdEnd = 0;
            while (cmdEnd < actualSpan.Length) {
                char curChar = actualSpan[cmdEnd];
                if (!char.IsLetterOrDigit(curChar) && curChar != '_' && curChar != '!') break;
                cmdEnd++;
            }

            if (cmdEnd == 0) {
                throw new ParsingException(parsingState.LineNumber, line, "Failed to tokenize command name");
            }

            return actualSpan[..cmdEnd];
        }

        public static ReadOnlySpan<char> GetActualSpanFromLine(string line) {
            int commentIndex = line.IndexOf('#');

            ReadOnlySpan<char> actualSpan = commentIndex >= 0 ? line.AsSpan(0, commentIndex) : line;
            return actualSpan.Trim();
        }

        private static void ReadContinuingText(ParsingState state, ref string text) {
            StringBuilder result = new();
            bool isTextContinued = true;

            while (isTextContinued && !state.IsEnded) {
                string textLine = state.MoveNext();
                result.Append(" ");
                result.Append(ReadText(textLine.AsSpan().Trim(), out isTextContinued));
            }

            if (result.Length > 0) {
                text += result.ToString();
            }
        }

        public static string ReadText(ReadOnlySpan<char> text, out bool isContinued) {
            isContinued = text.EndsWith("\\");
            Index endIndex = isContinued ? ^1 : ^0;
            return text[..endIndex].Trim().ToString();
        }

        public static void ValidateTokens(
            in LineTokens tokens,
            int minArgs,
            int maxArgs,
            bool requireText,
            bool optionalText = false) {
            int argCount = tokens.Arguments?.Length ?? 0;
            if (argCount < minArgs || argCount > maxArgs) {
                throw new ParsingException(
                    tokens.LineNumber,
                    tokens.OriginalLine,
                    $"Unexpected arg count for command {tokens.Command} - expected between {minArgs} and {maxArgs}");
            }

            if (!requireText && !optionalText && tokens.Text != null) {
                throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                           $"Unexpected text for command {tokens.Command}");
            } else if (requireText && tokens.Text == null) {
                throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                           $"Missing expected text for command {tokens.Command}");
            }
        }
    }
}
