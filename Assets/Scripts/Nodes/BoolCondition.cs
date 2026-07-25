using Game;

namespace Nodes {
    public class BoolCondition : VariableCondition {
        public bool Value { get; }

        public BoolCondition(Variable variable, bool value = true) : base(variable) {
            Value = value;
        }
        
        public override bool CheckCondition(TraversalState traversalState) {
            return traversalState.GetVariableValue<bool>(Variable) == Value;
        }
    }
}