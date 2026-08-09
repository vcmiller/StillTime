using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("elif")]
    public class ElseIfCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 1, 1, false);
            ElseIfCommand command = new(tokens.LineNumber, tokens.OriginalLine, tokens.Arguments);
            commands.Add(command);

            if (!string.IsNullOrEmpty(tokens.Text)) {
                state.Prepend(tokens.LineNumber, tokens.Text);
            }
        }
    }
}
