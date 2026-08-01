using System.Collections.Generic;
using StillTime.Nodes;

namespace StillTime.Commands {
    public class CostCommand : Command, ISequentialCommand {
        public int Cost { get; }

        public CostCommand(int lineNumber, string line, int cost) : base(lineNumber, line) {
            Cost = cost;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            nextNode.Cost += Cost;
        }
    }
}
