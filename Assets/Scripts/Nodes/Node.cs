using Game;

namespace Nodes {
    public abstract class Node : INode {
        public int Cost { get; set; }
    
        public string FullIdentifier { get; set; }

        public virtual string GetSelfIdentifier() {
            return GetType().Name;
        }

        public virtual void ApplyToState(ref MutableTraversalState state) { }
        public virtual void ApplyToStateAfterEnd(ref MutableTraversalState state, ref INode nextNode) { }
    }
}
