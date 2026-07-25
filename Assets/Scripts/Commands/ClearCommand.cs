using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class ClearCommand : Command {
        public ClearCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            ClearNode node = new();
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}