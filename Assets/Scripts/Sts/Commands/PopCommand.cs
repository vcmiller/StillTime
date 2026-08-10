using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class PopCommand : Command, ISequentialCommand {
        public bool IsTryPop { get; }

        public PopCommand(int lineNumber, string line, bool isTryPop) : base(lineNumber, line) {
            IsTryPop = isTryPop;
        }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            PopNode node = new(IsTryPop);
            builder.Append(node);
        }
    }
}
