using Game;

namespace Nodes {
    public class ResetRunNode : Node, ISingleNextNode {
        public INode Next { get; set; }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            
            state.ShowCountdown = false;
            state.CountdownValue = null;
            state.VisitedNodesCurrentRun.Clear();
            state.NodeForTimeout = null;
        }
    }
}