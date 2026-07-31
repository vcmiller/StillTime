using Game;

namespace Nodes {
    public interface IBranchOption {
        public string GetText(TraversalState state);
        public bool IsAvailable(TraversalState state);
        public INode GetNextNode(TraversalState state);
    }
}
