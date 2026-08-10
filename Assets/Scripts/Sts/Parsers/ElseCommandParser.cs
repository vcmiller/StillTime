using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("else")]
    public class ElseCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 0, 0, false, true);
            ElseCommand command = new(tokens.LineNumber, tokens.OriginalLine);
            commands.Add(command);

            if (!string.IsNullOrEmpty(tokens.Text)) {
                state.Prepend(tokens.LineNumber, tokens.Text);
            }
        }
    }
}
