using StillTime.Commands;

namespace StillTime.Parsers {
    [CustomCommandParser("if")]
    public class IfCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 100, false);

            IfCommand ifCommand = new(originalLineNumber, line, args);
            bool processingIfCommands = true;

            while (lineNumber < lines.Length) {
                Command subCommand = CommandParserDelegator.ParseCommand(lines, ref lineNumber);

                if (subCommand == null) continue;

                if (subCommand is ElseIfCommand elseIfCommand) {
                    processingIfCommands = false;
                    ifCommand.ElseIfCommands.Add(elseIfCommand);
                    continue;
                } else if (subCommand is ElseCommand elseCommand) {
                    ifCommand.ElseCommand = elseCommand;
                    break;
                } else if (subCommand is EndCommand) {
                    break;
                }

                if (!processingIfCommands) {
                    throw new ParsingException(subCommand.LineNumber, subCommand.Line,
                                               $"Expected only 'elif', 'else', or 'end'; found {subCommand}");
                }

                if (subCommand is not ISequentialCommand) {
                    throw new ParsingException(
                        subCommand.LineNumber,
                        subCommand.Line,
                        "Expected sequential command or 'elif', 'else', or 'end'");
                }

                ifCommand.Commands.Add(subCommand);

                if (subCommand is ISequenceTerminatingCommand { IsTerminating: true }) {
                    processingIfCommands = false;
                }
            }

            return ifCommand;
        }
    }
}
