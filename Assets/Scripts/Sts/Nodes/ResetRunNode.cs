using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class ResetRunNode : SequentialNode {
        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);

            state.ShowCountdown = false;
            state.CountdownValue = null;
            state.VisitedNodesCurrentRun.Clear();
            state.NodeForTimeout = null;
        }
    }
}
