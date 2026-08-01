using StillTime.Game;

namespace StillTime.Nodes {
    public interface ICondition {
        public bool CheckCondition(TraversalState traversalState);
    }
}