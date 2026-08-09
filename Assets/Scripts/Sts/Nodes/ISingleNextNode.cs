using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public interface ISingleNextNode : INode {
        public INode GetSingleNextNode(StateContainer state);
    }
}
