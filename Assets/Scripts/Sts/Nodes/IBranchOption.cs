using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public interface IBranchOption {
        public string GetText(StateContainer state);
        public bool IsAvailable(StateContainer state);
        public INode GetNextNode(StateContainer state);
    }
}
