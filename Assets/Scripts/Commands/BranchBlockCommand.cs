using System.Collections.Generic;

namespace Commands {
    public class BranchBlockCommand : TextCommand {
        public List<ChoiceCommand> Choices { get; } = new();
    }
}