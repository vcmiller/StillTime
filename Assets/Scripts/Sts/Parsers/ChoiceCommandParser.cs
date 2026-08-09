using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("choice")]
    public class ChoiceCommandParser : ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 1, 100, true);
            string[] conditions = tokens.Arguments[1..];
            ChoiceCommand choiceCommand = new(
                tokens.LineNumber,
                tokens.OriginalLine,
                tokens.Text,
                tokens.Arguments[0],
                conditions);

            commands.Add(choiceCommand);
        }
    }
}
