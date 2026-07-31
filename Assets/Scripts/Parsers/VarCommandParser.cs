using Commands;

namespace Parsers {
    [CustomCommandParser("var")]
    public class VarCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 3, 3, false);
            return new VarCommand(originalLineNumber, line, args[0], args[1], args[2]);
        }
    }
}
