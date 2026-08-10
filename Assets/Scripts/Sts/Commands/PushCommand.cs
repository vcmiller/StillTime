using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class PushCommand : Command, ISequentialCommand {
        public string TargetLabel { get; }

        public PushCommand(int lineNumber, string line, string targetLabel) : base(lineNumber, line) {
            TargetLabel = targetLabel;
        }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            INode target = graphData.GetNode(this, TargetLabel);
            PushNode node = new(target);
            builder.Append(node);
        }
    }
}
