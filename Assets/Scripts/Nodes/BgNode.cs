using UnityEngine;

namespace Nodes {
    public class BgNode : Node, ISingleNextNode {
        public INode Next { get; set; }
        
        public Color Color { get; }
        
        public float Time { get; }

        public BgNode(Color color, float time) {
            Color = color;
            Time = time;
        }
    }
}