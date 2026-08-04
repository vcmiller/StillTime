using System.Collections.Generic;
using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("elif")]
    public class ElseIfCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber];

            List<Command> commands = ParsingUtility.ParseCondBlockCommand(cmd, args, text, lines, ref lineNumber, true);

            ElseIfCommand command = new(originalLineNumber, line, args);
            command.Commands.AddRange(commands);

            return command;
        }
    }
}
