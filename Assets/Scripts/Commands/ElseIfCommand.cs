using System.Collections.Generic;

namespace Commands {
    public class ElseIfCommand : Command {
        public IReadOnlyList<string> Conditions { get; }

        public List<Command> Commands { get; } = new();

        public ElseIfCommand(int lineNumber, string line, IReadOnlyList<string> conditions) : base(lineNumber, line) {
            Conditions = conditions;
        }
    }
}
