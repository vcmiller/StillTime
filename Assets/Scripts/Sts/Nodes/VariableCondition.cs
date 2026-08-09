using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;

namespace StillTime.Sts.Nodes {
    public abstract class VariableCondition : ICondition {
        public Variable Variable { get; }

        protected VariableCondition(Variable variable) {
            Variable = variable;
        }

        public bool CheckCondition(StateContainer traversalState) {
            return CheckCondition(traversalState.GetOrCreate<VariablesComponent>());
        }

        protected abstract bool CheckCondition(VariablesComponent variables);
    }
}
