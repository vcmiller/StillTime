using Commands;

namespace Parsers {
    [CustomCommandParser("incr")]
    public class IncrVarCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 2, 2, false);
            int incrValue = int.TryParse(args[1], out int t)
                ? t
                : throw new ParsingException(originalLineNumber, line, $"Invalid int value {args[1]}");
            return new IncrVarCommand(originalLineNumber, line, args[0], incrValue);
        }
    }
}
