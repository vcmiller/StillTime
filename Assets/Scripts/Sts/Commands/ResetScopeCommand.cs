using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class ResetScopeCommand : Command, ISequentialCommand {
        public string Scope { get; }

        public ResetScopeCommand(int lineNumber, string line, string scope) : base(lineNumber, line) {
            Scope = scope;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode, Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            Scope scope = CommandUtility.GetResource<Scope>(this, Scope, resourceDictionary);
            ResetScopeNode node = new(scope);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}
