using System.Collections.Generic;

namespace Commands {
    public class ChoiceCommand : TextCommand {
        public string TargetLabel { get; }
        public IReadOnlyList<string> RequiredGates { get; }

        public ChoiceCommand(
            int lineNumber,
            string line,
            string text,
            string targetLabel,
            IReadOnlyList<string> requiredGates) :
            base(lineNumber, line, null, text) {

            TargetLabel = targetLabel;
            RequiredGates = requiredGates;
        }
    }
}