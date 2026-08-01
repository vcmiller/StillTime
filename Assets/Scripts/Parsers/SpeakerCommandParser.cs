using StillTime.Commands;
using UnityEngine;

namespace StillTime.Parsers {
    [CustomCommandParser("speaker")]
    public class SpeakerCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 2, 2, true);
            if (!ColorUtility.TryParseHtmlString("#" + args[1], out Color color)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid color value {args[1]}");
            }

            return new SpeakerCommand(originalLineNumber, line, args[0], color, text);
        }
    }
}
