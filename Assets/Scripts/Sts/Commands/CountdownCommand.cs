using System.Collections.Generic;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class CountdownCommand : Command, ISequentialCommand {
        public bool Show { get; }

        public int? Value { get; }

        public CountdownCommand(int lineNumber, string line, bool show, int? value) : base(lineNumber, line) {
            Show = show;
            Value = value;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            CountdownNode node = new(Show, Value);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}
