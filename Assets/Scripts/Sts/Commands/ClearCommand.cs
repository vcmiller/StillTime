using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class ClearCommand : Command, ISequentialCommand {
        public ClearCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public void ApplyToSequence(ref ISequentialNode nextNode,
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
