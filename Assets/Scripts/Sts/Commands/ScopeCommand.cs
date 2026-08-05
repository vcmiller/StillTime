using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class ScopeCommand : Command, IResourceCommand {
        public string Identifier { get; }

        public ScopeCommand(int lineNumber, string line, string identifier) : base(lineNumber, line) {
            Identifier = identifier;
        }

        public void CreateResources(Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary) {
            Scope scope = new(Identifier);
            resourceDictionary.Add(Identifier, scope);
        }
    }
}
