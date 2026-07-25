using Game;

namespace Nodes {
    public class TimeoutNode : Node, ISingleNextNode {
        public INode Next { get; set; }
        
        public INode TimeoutTarget { get; }
        
        public TimeoutNode(INode timeoutTarget) {
            TimeoutTarget = timeoutTarget;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.NodeForTimeout = TimeoutTarget;
        }
    }
}