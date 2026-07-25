using System.Collections.Generic;

namespace Commands {
    public class ChoiceCommand : TextCommand {
        public string TargetLabel { get; }
        public IReadOnlyList<string> Conditions { get; }
        public bool AlwaysAllow { get; }

        public ChoiceCommand(
            int lineNumber,
            string line,
            string text,
            string targetLabel,
            bool alwaysAllow,
            IReadOnlyList<string> conditions) :
            base(lineNumber, line, null, text) {

            TargetLabel = targetLabel;
            Conditions = conditions;
            AlwaysAllow = alwaysAllow;
        }
    }
}