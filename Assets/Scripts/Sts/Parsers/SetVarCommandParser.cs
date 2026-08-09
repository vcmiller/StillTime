using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("set")]
    public class SetVarCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 2, 2, false);
            SetVarCommand command = new(tokens.LineNumber, tokens.OriginalLine, tokens.Arguments[0],
                                        tokens.Arguments[1]);
            commands.Add(command);
        }
    }
}
