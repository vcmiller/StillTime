using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("incr")]
    public class IncrVarCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 2, 2, false);
            decimal incrValue = decimal.TryParse(tokens.Arguments[1], out decimal t)
                ? t
                : throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                             $"Invalid decimal value {tokens.Arguments[1]}");

            IncrVarCommand command = new(tokens.LineNumber, tokens.OriginalLine, tokens.Arguments[0], incrValue);
            commands.Add(command);
        }
    }
}
