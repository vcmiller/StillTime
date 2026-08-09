using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("delay")]
    public class DelayCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 1, 1, false);

            if (!float.TryParse(tokens.Arguments[0], out float delayTime)) {
                throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                           $"Invalid float value {tokens.Arguments[1]}");
            }

            commands.Add(new DelayCommand(tokens.LineNumber, tokens.OriginalLine, delayTime));
        }
    }
}
