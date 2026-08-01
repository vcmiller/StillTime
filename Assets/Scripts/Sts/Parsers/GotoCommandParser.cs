using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("goto")]
    [CustomCommandParser("goto_reset")]
    public class GotoCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 1, false);
            bool reset = cmd is "goto_reset";
            return new GotoCommand(originalLineNumber, line, args[0], reset);
        }
    }
}
