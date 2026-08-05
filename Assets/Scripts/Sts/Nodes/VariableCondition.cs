using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public abstract class VariableCondition : ICondition {
        public Variable Variable { get; }
        
        protected VariableCondition(Variable variable) {
            Variable = variable;
        }
        
        public abstract bool CheckCondition(TraversalState traversalState);
    }
}