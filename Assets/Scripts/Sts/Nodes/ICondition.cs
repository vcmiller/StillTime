using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public interface ICondition {
        public bool CheckCondition(TraversalState traversalState);
    }
}