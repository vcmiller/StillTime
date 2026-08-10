using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Expressions;
using StillTime.Sts.Nodes;
using StillTime.Sts.Parsers;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class InterruptCommand : Command, IResourceCommand {
        public string InterruptId { get; }
        public string TargetLabel { get; }
        public string Condition { get; }

        public InterruptCommand(
            int lineNumber,
            string line,
            string interruptId,
            string targetLabel,
            string condition) :
            base(lineNumber, line) {
            InterruptId = interruptId;
            TargetLabel = targetLabel;
            Condition = condition;
        }

        public void CreateResources(GraphData graphData) {
            graphData.Resources.Add(InterruptId, new Interrupt(InterruptId));
        }

        public void ValidateResources(GraphData graphData) {
            Interrupt interrupt = graphData.GetResource<Interrupt>(this, InterruptId);

            IExpression condition = ExpressionParser.ParseExpression(this, graphData, Condition);
            INode target = graphData.GetNode(this, TargetLabel);
            interrupt.Condition = condition;
            interrupt.TargetNode = target;
        }
    }
}
