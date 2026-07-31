using Commands;

namespace Parsers {
    [CustomCommandParser("set")]
    public class SetVarCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 2, 2, false);
            return new SetVarCommand(originalLineNumber, line, args[0], args[1]);
        }
    }
}
