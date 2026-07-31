using System;
using Commands;
using Game;

namespace Nodes {
    public class SetVariableNode : SequentialNode {
        public Variable Variable { get; }

        public object Value { get; }

        public SetVariableNode(Variable variable, object value) {
            if (value.GetType() != variable.DefaultValue.GetType()) {
                throw new Exception($"Invalid value for variable {variable.Identifier}: {value}");
            }

            Variable = variable;
            Value = value;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.SetVariableValue(Variable, Value);
        }
    }
}
