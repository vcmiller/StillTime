using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Expressions;
using StillTime.Sts.Nodes;
using StillTime.Sts.Parsers;

namespace StillTime.Sts.Commands {
    public class ChoiceCommand : TextCommand, IBranchSubCommand {
        public string TargetLabel { get; }
        public string Condition { get; }

        public ChoiceCommand(
            int lineNumber,
            string line,
            string text,
            string targetLabel,
            string condition = null) :
            base(lineNumber, line, null, text) {
            TargetLabel = targetLabel;
            Condition = condition;
        }

        public void CreateBranchOptions(GraphData graphData, List<IBranchOption> options) {
            INode targetNode = graphData.GetNode(this, TargetLabel);

            IExpression expression = string.IsNullOrEmpty(Condition)
                ? null
                : ExpressionParser.ParseExpression(this, graphData, Condition);
            
            Choice choice = new(Text, targetNode, expression);

            options.Add(choice);
        }
    }
}
