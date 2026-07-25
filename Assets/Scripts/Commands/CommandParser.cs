using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Commands {
    public static class CommandParser {
        public static List<Command> ParseScript(string scriptContent) {
            string[] lines = scriptContent.Split('\n');
            List<Command> commands = new();

            for (int i = 0; i < lines.Length; i++) {
                Command command = ParseCommand(lines, ref i);
                if (command == null) continue;

                if (command is ChoiceCommand or EndCommand) {
                    throw new ParsingException(i, lines[i], "Invalid command at root level");
                }

                commands.Add(command);
            }

            return commands;
        }

        private static Command ParseCommand(string[] lines, ref int index) {
            string line = lines[index];

            ReadOnlySpan<char> actualSpan = GetActualSpanFromLine(line);
            if (actualSpan.IsEmpty) return null;

            ReadCommandLine(line, index, out string cmd, out string text, out string[] args,
                out bool isTextContinued);

            while (isTextContinued && index < lines.Length - 1) {
                string textLine = lines[++index];
                text += " " + ReadText(textLine.AsSpan().Trim(), out isTextContinued);
            }

            switch (cmd) {
                case "label":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);
                    LabelBlockCommand labelCommand = new(index, line, args[0]);

                    while (index < lines.Length) {
                        index++;
                        if (index >= lines.Length) break;
                        int indexOfSubCommand = index;

                        Command labelSubCommand = ParseCommand(lines, ref index);
                        if (labelSubCommand == null) continue;
                        if (labelSubCommand is EndCommand) break;

                        if (labelSubCommand is ChoiceCommand or LabelBlockCommand or VarCommand) {
                            throw new ParsingException(indexOfSubCommand, lines[indexOfSubCommand],
                                "Invalid command inside label");
                        }

                        labelCommand.Commands.Add(labelSubCommand);

                        if (labelSubCommand is BranchBlockCommand or GotoCommand) break;
                    }

                    return labelCommand;
                case "branch":
                    ValidateCommand(line, index, cmd, args, text, 0, 1, true);
                    string branchSpeaker = args?.Length > 0 ? args[0] : null;
                    BranchBlockCommand branchCommand = new(index, line, branchSpeaker, text);

                    while (index < lines.Length) {
                        index++;
                        if (index >= lines.Length) break;
                        int indexOfSubCommand = index;

                        Command branchSubCommand = ParseCommand(lines, ref index);
                        if (branchSubCommand == null) continue;
                        if (branchSubCommand is EndCommand) break;

                        if (branchSubCommand is not ChoiceCommand choiceBranchSubCommand) {
                            throw new ParsingException(indexOfSubCommand, lines[indexOfSubCommand],
                                "Invalid command in branch");
                        }

                        branchCommand.Choices.Add(choiceBranchSubCommand);
                    }

                    return branchCommand;
                case "say":
                    ValidateCommand(line, index, cmd, args, text, 0, 1, true);
                    string saySpeaker = args?.Length > 0 ? args[0] : null;
                    return new SayCommand(index, line, saySpeaker, text);
                case "choice_always":
                case "choice":
                    ValidateCommand(line, index, cmd, args, text, 1, 100, true);
                    List<string> choiceConds = ReadArgsArray(args, 1);
                    bool alwaysAllow = cmd == "choice_always";
                    ChoiceCommand choiceCommand = new(index, line, text, args[0], alwaysAllow, choiceConds);

                    return choiceCommand;
                case "end":
                    ValidateCommand(line, index, cmd, args, text, 0, 0, false);
                    return new EndCommand(index, line);
                case "goto":
                case "goto_reset":
                    ValidateCommand(line, index, cmd, args, text, 1, 100, false);
                    bool reset = cmd is "goto_reset";
                    List<string> gotoConds = ReadArgsArray(args, 1);
                    return new GotoCommand(index, line, args[0], reset, gotoConds);
                case "timeout":
                    ValidateCommand(line, index, cmd, args, text, 0, 1, false);
                    return new TimeoutCommand(index, line, args?.Length > 0 ? args[0] : null);
                case "var":
                    ValidateCommand(line, index, cmd, args, text, 3, 3, false);
                    return new VarCommand(index, line, args[0], args[1], args[2]);
                case "set":
                    ValidateCommand(line, index, cmd, args, text, 2, 2, false);
                    return new SetVarCommand(index, line, args[0], args[1]);
                case "incr":
                    ValidateCommand(line, index, cmd, args, text, 2, 2, false);
                    int incrValue = int.TryParse(args[1], out int t)
                        ? t
                        : throw new ParsingException(index, line, $"Invalid int value {args[1]}");
                    return new IncrVarCommand(index, line, args[0], incrValue);
                case "cost":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);
                    if (!int.TryParse(args[0], out int cost)) {
                        throw new ParsingException(index, lines[index], $"Invalid cost value {args[0]}");
                    }

                    return new CostCommand(index, line, cost);
                case "speaker":
                    ValidateCommand(line, index, cmd, args, text, 2, 2, true);
                    if (!ColorUtility.TryParseHtmlString("#" + args[1], out Color color)) {
                        throw new ParsingException(index, lines[index], $"Invalid color value {args[1]}");
                    }

                    return new SpeakerCommand(index, line, args[0], color, text);
                case "countdown":
                    ValidateCommand(line, index, cmd, args, text, 1, 2, false);
                    if (!bool.TryParse(args[0], out bool show)) {
                        throw new ParsingException(index, lines[index], $"Invalid bool value {args[0]}");
                    }

                    int? value = null;
                    if (args.Length > 1) {
                        if (!int.TryParse(args[1], out int tempValue)) {
                            throw new ParsingException(index, lines[index], $"Invalid int value {args[1]}");
                        } else {
                            value = tempValue;
                        }
                    }

                    return new CountdownCommand(index, line, show, value);
                case "bg":
                    ValidateCommand(line, index, cmd, args, text, 1, 2, false);
                    if (!ColorUtility.TryParseHtmlString("#" + args[0], out Color bgColor)) {
                        throw new ParsingException(index, lines[index], $"Invalid color value {args[1]}");
                    }

                    float bgTime = 0;
                    if (args.Length > 1 && !float.TryParse(args[1], out bgTime)) {
                        throw new ParsingException(index, lines[index], $"Invalid float value {args[1]}");
                    }

                    return new BgCommand(index, line, bgColor, bgTime);
                case "delay":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);

                    if (!float.TryParse(args[0], out float delayTime)) {
                        throw new ParsingException(index, lines[index], $"Invalid float value {args[1]}");
                    }

                    return new DelayCommand(index, line, delayTime);
                case "clear":
                    ValidateCommand(line, index, cmd, args, text, 0, 0, false);
                    return new ClearCommand(index, line);
                default:
                    throw new ParsingException(index, lines[index], $"Unrecognized command {cmd}");
            }
        }

        private static List<string> ReadArgsArray(string[] args, int start) {
            List<string> results = new();
            for (int i = start; i < args.Length; i++) {
                results.Add(args[i]);
            }

            return results;
        }

        private static ReadOnlySpan<char> GetActualSpanFromLine(string line) {
            int commentIndex = line.IndexOf('#');

            ReadOnlySpan<char> actualSpan = commentIndex >= 0 ? line.AsSpan(0, commentIndex) : line;
            return actualSpan.Trim();
        }

        private static void ReadCommandLine(
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
            for (int i = 0; i < line.Length; i++) {
                char c = line[i];
                if (c == '_' || char.IsLetterOrDigit(c)) {
                    cmdEnd++;
                } else {
                    break;
                }
            }

            if (cmdEnd == 0) {
                throw new ParsingException(lineNumber, line, "Failed to parse command name");
            }

            cmd = line[..cmdEnd].ToString();
            text = null;
            args = null;
            isTextContinued = false;

            ReadOnlySpan<char> remaining = line[cmdEnd..].Trim();
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

        private static string ReadText(ReadOnlySpan<char> text, out bool isContinued) {
            isContinued = text.EndsWith("\\");
            Index endIndex = isContinued ? ^1 : ^0;
            return text[..endIndex].Trim().ToString();
        }

        private static void ValidateCommand(
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