using StillTime.Commands;

namespace StillTime.Parsers {
    [CustomCommandParser("label")]
    public class LabelBlockCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 1, false);
            LabelBlockCommand labelCommand = new(originalLineNumber, line, args[0]);

            while (lineNumber < lines.Length) {
                Command subCommand = CommandParserDelegator.ParseCommand(lines, ref lineNumber);

                if (subCommand == null) continue;

                if (subCommand is not ISequentialCommand) {
                    throw new ParsingException(
                        subCommand.LineNumber,
                        subCommand.Line,
                        "Invalid command inside label");
                }

                labelCommand.Commands.Add(subCommand);

                if (subCommand is ISequenceTerminatingCommand { IsTerminating: true }) {
                    break;
                }
            }

            return labelCommand;
        }
    }
}
