using StillTime.Game;

namespace StillTime.Nodes {
    public interface IBranchOption {
        public string GetText(TraversalState state);
        public bool IsAvailable(TraversalState state);
        public INode GetNextNode(TraversalState state);
    }
}
