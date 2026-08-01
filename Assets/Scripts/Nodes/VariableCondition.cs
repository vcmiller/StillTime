using StillTime.Game;

namespace StillTime.Nodes {
    public abstract class VariableCondition : ICondition {
        public Variable Variable { get; }
        
        protected VariableCondition(Variable variable) {
            Variable = variable;
        }
        
        public abstract bool CheckCondition(TraversalState traversalState);
    }
}