using System.Collections.Generic;
using StillTime.Nodes;

namespace StillTime.Commands {
    public class DelayCommand : Command, ISequentialCommand {
        public float Time { get; }

        public DelayCommand(int lineNumber, string line, float time) : base(lineNumber, line) {
            Time = time;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            DelayNode node = new(Time);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}
