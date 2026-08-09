using StillTime.Sts.Resources;
using StillTime.Sts.Runtime.Components;

namespace StillTime.Sts.Nodes {
    public class BoolCondition : VariableCondition {
        public bool Value { get; }

        public BoolCondition(Variable variable, bool value = true) : base(variable) {
            Value = value;
        }

        protected override bool CheckCondition(VariablesComponent variables) {
            return variables.GetVariableValue(Variable).ToBool() == Value;
        }
    }
}
