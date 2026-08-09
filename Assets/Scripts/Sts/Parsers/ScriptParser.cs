using System.Collections.Generic;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;

namespace StillTime.Sts.Parsers {
    public static class ScriptParser {
        public static List<ICommand> ParseScript(string scriptContent) {
            string[] lines = scriptContent.Split('\n');
            List<ICommand> commands = new();
            ParsingState state = new(lines, 0);

            while (!state.IsEnded) {
                CommandParserDelegator.ParseLine(state, commands);
            }

            return commands;
        }
    }
}
