using StillTime.Commands;

namespace StillTime.Parsers {
    [CustomCommandParser("say")]
    public class SayCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ReadContinuingText(lines, ref lineNumber, ref text, isTextContinued);

            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 0, 1, true);
            string saySpeaker = args?.Length > 0 ? args[0] : null;
            return new SayCommand(originalLineNumber, line, saySpeaker, text);
        }
    }
}
