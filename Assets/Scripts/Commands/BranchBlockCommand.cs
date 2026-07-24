using System.Collections.Generic;

namespace Commands {
    public class BranchBlockCommand : TextCommand {
        public List<ChoiceCommand> Choices { get; } = new();

        public BranchBlockCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }
    }
}