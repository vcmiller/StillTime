using StillTime.Commands;
using UnityEngine;

namespace StillTime.Parsers {
    [CustomCommandParser("bg")]
    public class BgCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];

            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 1, 2, false);
            if (!ColorUtility.TryParseHtmlString("#" + args[0], out Color bgColor)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid color value {args[1]}");
            }

            float bgTime = 0;
            if (args.Length > 1 && !float.TryParse(args[1], out bgTime)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid float value {args[1]}");
            }

            return new BgCommand(originalLineNumber, line, bgColor, bgTime);
        }
    }
}
