using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("pop")]
    [CustomCommandParser("trypop")]
    public class PopCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 0, 0, false);
            commands.Add(new PopCommand(tokens.LineNumber, tokens.OriginalLine, tokens.Command == "trypop"));
        }
    }
}
