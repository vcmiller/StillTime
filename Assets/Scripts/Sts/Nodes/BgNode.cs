using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
    public class BgNode : SequentialNode {
        public StsColor Color { get; }

        public float Time { get; }

        public BgNode(StsColor color, float time) {
            Color = color;
            Time = time;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.BgColor = Color;
        }
    }
}
