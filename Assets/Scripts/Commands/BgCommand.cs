using System.Collections.Generic;
using StillTime.Nodes;
using UnityEngine;

namespace StillTime.Commands {
    public class BgCommand : Command, ISequentialCommand {
        public Color Color { get; }
        public float Time { get; }

        public BgCommand(int lineNumber, string line, Color color, float time) : base(lineNumber, line) {
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
