using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("interrupt")]
    public class InterruptCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 3, 100, false);
            InterruptCommand command = new(tokens.LineNumber, tokens.OriginalLine, tokens.Arguments[0],
                                           tokens.Arguments[1], tokens.Arguments[2..]);
            commands.Add(command);
        }
    }
}
