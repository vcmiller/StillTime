using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("scope")]
    public class ScopeCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 1, false);
            return new ScopeCommand(lineNumber, line, args[0]);
        }
    }
}
