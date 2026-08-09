using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    public interface ICommandParser {
        public void ParseCommand(ParsingState state, List<ICommand> commands);
    }
}
