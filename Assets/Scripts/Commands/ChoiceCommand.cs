using System.Collections.Generic;

namespace Commands {
    public class ChoiceCommand : TextCommand {
        public string TargetLabel { get; set; }
        public List<string> RequiredGates { get; } = new();
    }
}