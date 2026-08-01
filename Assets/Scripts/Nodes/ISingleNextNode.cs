using StillTime.Game;

namespace StillTime.Nodes {
    public interface ISingleNextNode : INode {
        public INode GetSingleNextNode(TraversalState state);
    }
}
