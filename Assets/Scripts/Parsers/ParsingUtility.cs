using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Commands;

namespace Parsers {
    public static class ParsingUtility {
        public static ReadOnlySpan<char> GetActualSpanFromLine(string line) {
            int commentIndex = line.IndexOf('#');

            ReadOnlySpan<char> actualSpan = commentIndex >= 0 ? line.AsSpan(0, commentIndex) : line;
            return actualSpan.Trim();
        }

        public static void ReadCommandLine(
            string line,
            int lineNumber,
            out string cmd,
            out string text,
            out string[] args,
            out bool isTextContinued) {
            ReadOnlySpan<char> actualLineSpan = GetActualSpanFromLine(line);
            if (actualLineSpan.IsEmpty) {
                throw new Exception("Unexpected state");
            }

            int cmdEnd = 0;
            for (int i = 0; i < actualLineSpan.Length; i++) {
                char c = actualLineSpan[i];
                if (c == '_' || char.IsLetterOrDigit(c)) {
                    cmdEnd++;
                } else {
                    break;
                }
            }

            if (cmdEnd == 0) {
                throw new ParsingException(lineNumber, line, "Failed to parse command name");
            }

            cmd = actualLineSpan[..cmdEnd].ToString();
            text = null;
            args = null;
            isTextContinued = false;

            ReadOnlySpan<char> remaining = actualLineSpan[cmdEnd..].Trim();
            if (remaining.StartsWith("(")) {
                int indexOfClose = remaining.IndexOf(')');
                if (indexOfClose < 0) {
                    throw new ParsingException(lineNumber, line, "Encounter '(' without ')'");
                }

                args = remaining[1..indexOfClose].ToString().Split(',').Select(s => s.Trim()).ToArray();
                remaining = remaining[(indexOfClose + 1)..].Trim();
            }

            if (remaining.IsEmpty) return;

            if (!remaining.StartsWith(":")) {
                throw new ParsingException(lineNumber, line, "Expected ':' before text");
            }

            text = ReadText(remaining[1..], out isTextContinued);
        }

        public static void ReadContinuingText(string[] lines, ref int lineNumber, ref string text,
                                              bool isTextContinued) {
            if (!isTextContinued) return;

            StringBuilder result = new();
            while (isTextContinued && lineNumber < lines.Length) {
                string textLine = lines[lineNumber++];
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

        public static void ValidateCommand(
            string line,
            int lineNumber,
            string cmd,
            string[] args,
            string text,
            int minArgs,
            int maxArgs,
            bool expectText) {
            int argCount = args?.Length ?? 0;
            if (argCount < minArgs || argCount > maxArgs) {
                throw new ParsingException(lineNumber, line,
                                           $"Unexpected arg count for command {cmd} - expected between {minArgs} and {maxArgs}");
            }

            if (!expectText && text != null) {
                throw new ParsingException(lineNumber, line, $"Unexpected text for command {cmd}");
            } else if (expectText && text == null) {
                throw new ParsingException(lineNumber, line, $"Missing expected text for command {cmd}");
            }
        }
    }
}
