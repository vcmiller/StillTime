using System;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
    public class SetVariableNode : SequentialNode {
        public Variable Variable { get; }

        public StsValue Value { get; }

        public SetVariableNode(Variable variable, StsValue value) {
            if (value.ValueType != variable.Type) {
                throw new Exception($"Invalid value for variable {variable.Identifier}: {value}");
            }

            Variable = variable;
            Value = value;
        }

        public override void ApplyAfterAdvanceToSelf(GameGraph graph, StateContainer state) {
            state.GetOrCreate<VariablesComponent>().SetVariableValue(Variable, Value);
        }
    }
}
