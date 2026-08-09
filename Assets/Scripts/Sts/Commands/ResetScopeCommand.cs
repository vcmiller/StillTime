using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class ResetScopeCommand : Command, ISequentialCommand {
        public string Scope { get; }

        public ResetScopeCommand(int lineNumber, string line, string scope) : base(lineNumber, line) {
            Scope = scope;
        }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            Scope scope = graphData.GetResource<Scope>(this, Scope);
            ResetScopeNode node = new(scope);
            builder.Append(node);
        }
    }
}
