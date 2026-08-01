using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    public interface ICommandParser {
        public Command ParseCommand(string[] lines,
                                    ref int lineNumber,
                                    string cmd,
                                    string[] args,
                                    string text, bool isTextContinued);
    }
}
