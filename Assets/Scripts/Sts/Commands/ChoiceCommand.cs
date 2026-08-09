using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class ChoiceCommand : TextCommand, IBranchSubCommand {
        public string TargetLabel { get; }
        public IReadOnlyList<string> Conditions { get; }

        public ChoiceCommand(
            int lineNumber,
            string line,
            string text,
            string targetLabel,
            IReadOnlyList<string> conditions) :
            base(lineNumber, line, null, text) {
            TargetLabel = targetLabel;
            Conditions = conditions;
        }

        public void CreateBranchOptions(GraphData graphData, List<IBranchOption> options) {
            INode targetNode = graphData.GetNode(this, TargetLabel);
            Choice choice = new(Text, targetNode);

            foreach (string condStr in Conditions) {
                ICondition cond = CommandUtility.ProcessCondition(this, condStr, graphData);
                choice.Conditions.Add(cond);
            }

            options.Add(choice);
        }
    }
}
