using Game;

namespace Nodes {
    public interface ICondition {
        public bool CheckCondition(TraversalState traversalState);
    }
}