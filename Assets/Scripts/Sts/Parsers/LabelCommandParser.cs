using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("label")]
    public class LabelCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 1, 1, false);
            LabelCommand labelCommand = new(tokens.LineNumber, tokens.OriginalLine, tokens.Arguments[0]);
            commands.Add(labelCommand);
        }
    }
}
