using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("countdown")]
    public class CountdownCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 2, false);
            if (!bool.TryParse(args[0], out bool show)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid bool value {args[0]}");
            }

            int? value = null;
            if (args.Length > 1) {
                if (!int.TryParse(args[1], out int tempValue)) {
                    throw new ParsingException(originalLineNumber, line, $"Invalid int value {args[1]}");
                } else {
                    value = tempValue;
                }
            }

            return new CountdownCommand(originalLineNumber, line, show, value);
        }
    }
}
