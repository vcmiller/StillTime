using System.Collections.Generic;
using Nodes;
using UnityEngine;

namespace Commands {
    public class BgCommand : Command {
        public Color Color { get; }
        public float Time { get; }

        public BgCommand(int lineNumber, string line, Color color, float time) : base(lineNumber, line) {
            Color = color;
            Time = time;
        }

        public override void ApplyToSequence(
            ref ISingleNextNode nextNode,
            IReadOnlyDictionary<string, Resource> resources,
            IReadOnlyDictionary<string, INode> nodeDictionary,
            List<INode> createdNodes) {
            
            BgNode node = new(Color, Time);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}