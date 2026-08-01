using System.Collections.Generic;
using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    public static class ScriptParser {
        public static List<Command> ParseScript(string scriptContent) {
            string[] lines = scriptContent.Split('\n');
            List<Command> commands = new();

            for (int i = 0; i < lines.Length;) {
                Command command = CommandParserDelegator.ParseCommand(lines, ref i);
                if (command == null) continue;

                if (command is not (ISequentialCommand or IResourceCommand)) {
                    throw new ParsingException(command.LineNumber, command.Line, "Invalid command at root level");
                }

                commands.Add(command);
            }

            return commands;
        }
    }
}
