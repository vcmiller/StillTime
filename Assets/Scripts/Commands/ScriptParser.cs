using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Commands {
    public static class ScriptParser {
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
                        Command labelSubCommand = ParseCommand(lines, ref index);
                        if (labelSubCommand == null) continue;
                        if (labelSubCommand is EndCommand) break;

                        if (labelSubCommand is ChoiceCommand or LabelBlockCommand or GateCommand) {
                            throw new ParsingException(index, lines[index], "Invalid command inside label");
                        }

                        labelCommand.Commands.Add(labelSubCommand);

                        if (labelSubCommand is BranchBlockCommand or GotoCommand) break;
                    }

                    return labelCommand;
                case "branch":
                    ValidateCommand(line, index, cmd, args, text, 0, 0, true);
                    BranchBlockCommand branchCommand = new(index, line, text);
                    
                    while (index < lines.Length) {
                        index++;
                        Command branchSubCommand = ParseCommand(lines, ref index);
                        if (branchSubCommand == null) continue;
                        if (branchSubCommand is EndCommand) break;

                        if (branchSubCommand is not ChoiceCommand choiceBranchSubCommand) {
                            throw new ParsingException(index, lines[index], "Invalid command in branch");
                        }

                        branchCommand.Choices.Add(choiceBranchSubCommand);
                    }

                    return branchCommand;
                case "say":
                    ValidateCommand(line, index, cmd, args, text, 0, 0, true);
                    return new SayCommand(index, line, text);
                case "choice":
                    ValidateCommand(line, index, cmd, args, text, 1, 100, true);

                    List<string> requiredGates = new();
                    for (int i = 1; i < args.Length; i++) {
                        requiredGates.Add(args[i]);
                    }

                    ChoiceCommand choiceCommand = new(index, line, text, args[0], requiredGates);

                    return choiceCommand;
                case "end":
                    ValidateCommand(line, index, cmd, args, text, 0, 0, false);
                    return new EndCommand(index, line);
                case "goto":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);
                    return new GotoCommand(index, line, args[0]);
                case "gate":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);
                    return new GateCommand(index, line, args[0]);
                case "unlock":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);
                    return new UnlockCommand(index, line, args[0]);
                case "cost":
                    ValidateCommand(line, index, cmd, args, text, 1, 1, false);
                    if (!float.TryParse(args[0], out float cost)) {
                        throw new ParsingException(index, lines[index], $"Invalid cost value {args[0]}");
                    }

                    return new CostCommand(index, line, cost);
                default:
                    throw new ParsingException(index, lines[index], $"Unrecognized command {cmd}");
            }
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