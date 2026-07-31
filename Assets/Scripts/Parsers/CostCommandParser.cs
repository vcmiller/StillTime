using Commands;

namespace Parsers {
    [CustomCommandParser("cost")]
    public class CostCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 1, false);
            if (!int.TryParse(args[0], out int cost)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid cost value {args[0]}");
            }

            return new CostCommand(originalLineNumber, line, cost);
        }
    }
}
