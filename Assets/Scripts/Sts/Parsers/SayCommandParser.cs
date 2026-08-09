using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("say")]
    public class SayCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 0, 1, true);
            string saySpeaker = tokens.Arguments?.Length > 0 ? tokens.Arguments[0] : null;
            SayCommand sayCommand = new(tokens.LineNumber, tokens.OriginalLine, tokens.Text, saySpeaker);
            commands.Add(sayCommand);
        }
    }
}
