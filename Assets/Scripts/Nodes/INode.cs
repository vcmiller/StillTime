using Game;

namespace Nodes {
    public interface INode {
        int Cost { get; set; }
        
        string FullIdentifier { get; set; }

        public string GetSelfIdentifier();

        public void ApplyToState(ref MutableTraversalState state);
    }
}