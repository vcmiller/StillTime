using StillTime.Commands;

namespace StillTime.Parsers {
    [CustomCommandParser("elif")]
    public class ElseIfCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 100, false);

            ElseIfCommand elseIfCommand = new(originalLineNumber, line, args);

            while (lineNumber < lines.Length) {
                int lineNumberBefore = lineNumber;
                Command subCommand = CommandParserDelegator.ParseCommand(lines, ref lineNumber);

                if (subCommand == null) continue;

                if (subCommand is ElseIfCommand or ElseCommand or EndCommand) {
                    lineNumber = lineNumberBefore;
                    break;
                }

                if (subCommand is not ISequentialCommand) {
                    throw new ParsingException(
                        subCommand.LineNumber,
                        subCommand.Line,
                        "Expected sequential command or 'elif', 'else', or 'end'");
                }

                elseIfCommand.Commands.Add(subCommand);

                if (subCommand is ISequenceTerminatingCommand { IsTerminating: true }) {
                    break;
                }
            }

            return elseIfCommand;
        }
    }
}
