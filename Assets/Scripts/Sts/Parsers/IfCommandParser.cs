using System.Collections.Generic;
using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("if")]
    public class IfCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber];

            List<Command> commands = ParsingUtility.ParseCondBlockCommand(cmd, args, text, lines, ref lineNumber, true);

            IfCommand ifCommand = new(originalLineNumber, line, args);
            ifCommand.Commands.AddRange(commands);

            while (lineNumber < lines.Length) {
                int cmdOriginalLineNumber = lineNumber;
                Command subCommand = CommandParserDelegator.ParseCommand(lines, ref lineNumber);

                if (subCommand == null) continue;

                if (subCommand is ElseIfCommand elseIfCommand) {
                    ifCommand.ElseIfCommands.Add(elseIfCommand);
                    continue;
                } else if (subCommand is ElseCommand elseCommand) {
                    ifCommand.ElseCommand = elseCommand;
                    break;
                }

                lineNumber = cmdOriginalLineNumber;
                break;
            }

            return ifCommand;
        }
    }
}
