using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class ScopeCommand : Command, IResourceCommand {
        public string Identifier { get; }

        public ScopeCommand(int lineNumber, string line, string identifier) : base(lineNumber, line) {
            Identifier = identifier;
        }

        public void CreateResources(GraphData graphData) {
            Scope scope = new(Identifier);
            graphData.Resources.Add(Identifier, scope);
        }
    }
}
