using StillTime.Commands;

namespace StillTime.Parsers {
    [CustomCommandParser("else")]
    public class ElseCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 0, 0, false);

            ElseCommand elseCommand = new(originalLineNumber, line);

            while (lineNumber < lines.Length) {
                Command subCommand = CommandParserDelegator.ParseCommand(lines, ref lineNumber);

                if (subCommand == null) continue;

                if (subCommand is EndCommand) {
                    break;
                }

                if (subCommand is not ISequentialCommand) {
                    throw new ParsingException(
                        subCommand.LineNumber,
                        subCommand.Line,
                        "Expected sequential command or 'end'");
                }

                elseCommand.Commands.Add(subCommand);

                if (subCommand is ISequenceTerminatingCommand { IsTerminating: true }) {
                    break;
                }
            }

            return elseCommand;
        }
    }
}
