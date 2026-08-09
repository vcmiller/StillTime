using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class GotoCommand : Command, ISequentialCommand {
        public string TargetLabel { get; }

        public GotoCommand(int lineNumber, string line, string targetLabel) :
            base(lineNumber, line) {
            TargetLabel = targetLabel;
        }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            INode target = graphData.GetNode(this, TargetLabel);
            builder.EndWithExternalNode(target);
        }
    }
}
