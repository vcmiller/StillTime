using Commands;

namespace Parsers {
    [CustomCommandParser("goto")]
    [CustomCommandParser("goto_reset")]
    public class GotoCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 100, false);
            bool reset = cmd is "goto_reset";
            string[] conditions = args[1..];
            return new GotoCommand(originalLineNumber, line, args[0], reset, conditions);
        }
    }
}
