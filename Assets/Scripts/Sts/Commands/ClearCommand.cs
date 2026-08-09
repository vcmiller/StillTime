using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class ClearCommand : Command, ISequentialCommand {
        public ClearCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            builder.Append(new ClearNode());
        }
    }
}
