using StillTime.Commands;

namespace StillTime.Parsers {
    [CustomCommandParser("delay")]
    public class DelayCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 1, false);

            if (!float.TryParse(args[0], out float delayTime)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid float value {args[1]}");
            }

            return new DelayCommand(originalLineNumber, line, delayTime);
        }
    }
}
