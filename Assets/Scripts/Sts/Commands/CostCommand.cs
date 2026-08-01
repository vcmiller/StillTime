using System.Collections.Generic;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
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
