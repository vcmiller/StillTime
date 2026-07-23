using System.Collections.Generic;

namespace Commands {
    public class LabelBlockCommand : Command {
        public string Identifier { get; }

        public List<Command> Commands { get; } = new();

        public LabelBlockCommand(int lineNumber, string line, string identifier) : base(lineNumber, line) {
            Identifier = identifier;
        }
    }
}