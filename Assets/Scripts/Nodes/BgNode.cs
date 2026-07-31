using Game;
using UnityEngine;

namespace Nodes {
    public class BgNode : SequentialNode {
        public Color Color { get; }

        public float Time { get; }

        public BgNode(Color color, float time) {
            Color = color;
            Time = time;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.BgColor = Color;
        }
    }
}
