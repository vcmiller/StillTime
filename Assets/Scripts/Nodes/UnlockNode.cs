using Game;

namespace Nodes {
    public class UnlockNode : Node, ISingleNextNode {
        public INode Next { get; set; }
    
        public Gate Gate { get; }
        
        public UnlockNode(Gate gate) {
            Gate = gate;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.UnlockedGates.Add(Gate);
        }
    }
}