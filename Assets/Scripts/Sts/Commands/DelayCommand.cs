using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
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

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            builder.Append(new DelayNode(Time));
        }
    }
}
