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
                    throw new Exception($"Invalid command type for root level: {command}");
                }
                
                commands.Add(command);
            }

            return commands;
        }

        private static Command ParseCommand(string[] lines, ref int index) {
            string line = lines[index];

            ReadOnlySpan<char> actualSpan = GetActualSpanFromLine(line);
            if (actualSpan.IsEmpty) return null;
                
            ReadCommandLine(actualSpan, out string cmd, out string text, out string[] args, 
                out bool isTextContinued);

            while (isTextContinued && index < lines.Length - 1) {
                string textLine = lines[++index];
                text += " " + ReadText(textLine.AsSpan().Trim(), out isTextContinued);
            }

            switch (cmd) {
                case "label":
                    ValidateCommand(cmd, args, text, 1, 1, false);
                    LabelBlockCommand labelCommand = new() { Identifier = args[0] };

                    while (index < lines.Length) {
                        index++;
                        Command labelSubCommand = ParseCommand(lines, ref index);
                        if (labelSubCommand == null) continue;
                        if (labelSubCommand is EndCommand) break;

                        if (labelSubCommand is ChoiceCommand or LabelBlockCommand or GateCommand) {
                            throw new Exception($"Invalid command type in label: {labelSubCommand}");
                        }

                        labelCommand.Commands.Add(labelSubCommand);

                        if (labelSubCommand is BranchBlockCommand or GotoCommand) break;
                    }

                    return labelCommand;
                case "branch":
                    ValidateCommand(cmd, args, text, 0, 0, true);
                    BranchBlockCommand branchCommand = new() { Text = text };
                    
                    while (index < lines.Length) {
                        index++;
                        Command branchSubCommand = ParseCommand(lines, ref index);
                        if (branchSubCommand == null) continue;
                        if (branchSubCommand is EndCommand) break;

                        if (branchSubCommand is not ChoiceCommand choiceBranchSubCommand) {
                            throw new Exception($"Invalid command type in label: {branchSubCommand}");
                        }

                        branchCommand.Choices.Add(choiceBranchSubCommand);
                    }

                    return branchCommand;
                case "say":
                    ValidateCommand(cmd, args, text, 0, 0, true);
                    return new SayCommand { Text = text };
                case "choice":
                    ValidateCommand(cmd, args, text, 1, 100, true);
                    ChoiceCommand choiceCommand = new() { TargetLabel = args[0], Text = text };
                    for (int i = 1; i < args.Length; i++) {
                        choiceCommand.RequiredGates.Add(args[i]);
                    }

                    return choiceCommand;
                case "end":
                    ValidateCommand(cmd, args, text, 0, 0, false);
                    return new EndCommand();
                case "goto":
                    ValidateCommand(cmd, args, text, 1, 1, false);
                    return new GotoCommand { TargetLabel = args[0] };
                case "gate":
                    ValidateCommand(cmd, args, text, 1, 1, false);
                    return new GateCommand { Name = args[0] };
                case "unlock":
                    ValidateCommand(cmd, args, text, 1, 1, false);
                    return new UnlockCommand { Gate = args[0] };
                case "cost":
                    ValidateCommand(cmd, args, text, 1, 1, false);
                    if (!float.TryParse(args[0], out float cost)) {
                        throw new Exception("Failed to parse cost for cost cmd.");
                    }

                    return new CostCommand { Cost = cost };
                default:
                    throw new Exception($"Unrecognized command {cmd}");
            }
        }

        private static ReadOnlySpan<char> GetActualSpanFromLine(string line) {
            int commentIndex = line.IndexOf('#');

            ReadOnlySpan<char> actualSpan = commentIndex >= 0 ? line.AsSpan(0, commentIndex) : line;
            return actualSpan.Trim();
        }

        private static void ReadCommandLine(
            ReadOnlySpan<char> line,
            out string cmd,
            out string text,
            out string[] args,
            out bool isTextContinued) {

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
                throw new Exception($"Failed to parse command '{line.ToString()}'");
            }

            cmd = line[..cmdEnd].ToString();
            text = null;
            args = null;
            isTextContinued = false;

            ReadOnlySpan<char> remaining = line[cmdEnd..].Trim();
            if (remaining.StartsWith("(")) {
                int indexOfClose = remaining.IndexOf(')');
                if (indexOfClose < 0) {
                    throw new Exception($"Failed to parse command '{line.ToString()}'");
                }

                args = remaining[1..indexOfClose].ToString().Split(',').Select(s => s.Trim()).ToArray();
                remaining = remaining[(indexOfClose + 1)..].Trim();
            }
            
            if (remaining.IsEmpty) return;
            
            if (!remaining.StartsWith(":")) {
                throw new Exception($"Failed to parse command '{line.ToString()}'");
            }

            text = ReadText(remaining[1..], out isTextContinued);
        }

        private static string ReadText(ReadOnlySpan<char> text, out bool isContinued) {
            isContinued = text.EndsWith("\\");
            Index endIndex = isContinued ? ^1 : ^0;
            return text[..endIndex].Trim().ToString();
        }

        private static void ValidateCommand(
            string cmd, string[] args, string text, int minArgs, int maxArgs, bool expectText) {

            int argCount = args?.Length ?? 0;
            if (argCount < minArgs || argCount > maxArgs) {
                throw new Exception($"Unexpected arg count for cmd {cmd}: {string.Join(", ", args ?? Array.Empty<string>())}");
            }

            if (!expectText && text != null) {
                throw new Exception($"Unexpected text for cmd {cmd}: {text}");
            } else if (expectText && text == null) {
                throw new Exception($"Expected text for cmd {cmd}.");
            }
        }
    }
}