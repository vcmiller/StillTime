using System.Collections.Generic;

namespace Commands {
    public class ElseCommand : Command {
        public List<Command> Commands { get; } = new();

        public ElseCommand(int lineNumber, string line) : base(lineNumber, line) { }
    }
}
