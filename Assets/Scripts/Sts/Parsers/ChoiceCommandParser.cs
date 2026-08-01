using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("choice")]
    [CustomCommandParser("choice_always")]
    public class ChoiceCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ReadContinuingText(lines, ref lineNumber, ref text, isTextContinued);

            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 100, true);
            string[] conditions = args[1..];
            bool alwaysAllow = cmd == "choice_always";
            ChoiceCommand choiceCommand = new(originalLineNumber, line, text, args[0], alwaysAllow, conditions);

            return choiceCommand;
        }
    }
}
