using StillTime.Sts.Commands;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("speaker")]
    public class SpeakerCommandParser : ICommandParser {
        public Command ParseCommand(string[] lines, ref int lineNumber, string cmd, string[] args, string text,
                                    bool isTextContinued) {
            int originalLineNumber = lineNumber;
            string line = lines[lineNumber++];
            ParsingUtility.ValidateCommand(line, originalLineNumber, cmd, args, text, 2, 2, true);
            if (!StsColor.TryParseHex(args[1], out StsColor color)) {
                throw new ParsingException(originalLineNumber, line, $"Invalid color value {args[1]}");
            }

            return new SpeakerCommand(originalLineNumber, line, args[0], color, text);
        }
    }
}
