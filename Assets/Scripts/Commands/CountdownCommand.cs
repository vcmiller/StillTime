using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class CountdownCommand : Command {
        public bool Show { get; }

        public int? Value { get; }

        public CountdownCommand(int lineNumber, string line, bool show, int? value) : base(lineNumber, line) {
            Show = show;
            Value = value;
        }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            CountdownNode node = new(Show, Value);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}