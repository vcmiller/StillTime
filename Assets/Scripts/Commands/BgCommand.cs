using System.Collections.Generic;
using Nodes;
using UnityEngine;

namespace Commands {
    public class BgCommand : Command, ISequentialCommand {
        public Color Color { get; }
        public float Time { get; }

        public BgCommand(int lineNumber, string line, Color color, float time) : base(lineNumber, line) {
            Color = color;
            Time = time;
        }

        public void ApplyToSequence(
            ref ISingleNextNode nextNode,
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
