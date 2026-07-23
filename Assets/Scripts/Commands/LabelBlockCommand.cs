using System.Collections.Generic;

namespace Commands {
    public class LabelBlockCommand : Command {
        public string Identifier { get; set; }

        public List<Command> Commands { get; } = new();
    }
}