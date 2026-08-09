using System.Collections.Generic;

namespace StillTime.Sts.Commands {
    public class InterruptCommand : Command {
        public string InterruptId { get; }
        public string TargetLabel { get; }
        public IReadOnlyList<string> Conditions { get; }

        public InterruptCommand(
            int lineNumber,
            string line,
            string interruptId,
            string targetLabel,
            IReadOnlyList<string> conditions) :
            base(lineNumber, line) {
            InterruptId = interruptId;
            TargetLabel = targetLabel;
            Conditions = conditions;
        }
    }
}
