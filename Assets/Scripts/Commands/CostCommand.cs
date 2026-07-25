using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class CostCommand : Command {
        public int Cost { get; }

        public CostCommand(int lineNumber, string line, int cost) : base(lineNumber, line) {
            Cost = cost;
        }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            nextNode.Cost += Cost;
        }
    }
}