using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("branch")]
    public class BranchBlockCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ReadContinuingText(lines, ref lineNumber, ref text, isTextContinued);

            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 0, 1, true);

            string branchSpeaker = args?.Length > 0 ? args[0] : null;
            BranchBlockCommand branchCommand = new(originalLineNumber, line, branchSpeaker, text);

            while (lineNumber < lines.Length) {
                Command subCommand = CommandParserDelegator.ParseCommand(lines, ref lineNumber);

                if (subCommand == null) continue;
                if (subCommand is EndCommand) break;

                if (subCommand is not IBranchSubCommand branchSubCommand) {
                    throw new ParsingException(
                        subCommand.LineNumber,
                        subCommand.Line,
                        "Invalid command in branch");
                }

                branchCommand.SubCommands.Add(branchSubCommand);
            }

            return branchCommand;
        }
    }
}
