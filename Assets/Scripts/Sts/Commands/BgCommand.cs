using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Commands {
    public class BgCommand : Command, ISequentialCommand {
        public StsColor Color { get; }
        public float Time { get; }

        public BgCommand(int lineNumber, string line, StsColor color, float time) : base(lineNumber, line) {
            Color = color;
            Time = time;
        }

        public void ApplyToSequence(
            ref ISequentialNode nextNode,
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary,
            List<INode> createdNodes) {

            BgNode node = new(Color, Time);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}
