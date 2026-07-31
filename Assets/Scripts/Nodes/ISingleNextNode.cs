using Game;

namespace Nodes {
    public interface ISingleNextNode : INode {
        public INode GetSingleNextNode(TraversalState state);
    }
}
