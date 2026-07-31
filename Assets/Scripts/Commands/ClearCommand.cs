using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class ClearCommand : Command, ISequentialCommand {
        public ClearCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public void ApplyToSequence(ref ISingleNextNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            ClearNode node = new();
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}
