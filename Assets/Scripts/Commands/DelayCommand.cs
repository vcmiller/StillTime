using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class DelayCommand : Command {
        public float Time { get; }

        public DelayCommand(int lineNumber, string line, float time) : base(lineNumber, line) {
            Time = time;
        }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            DelayNode node = new(Time);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}