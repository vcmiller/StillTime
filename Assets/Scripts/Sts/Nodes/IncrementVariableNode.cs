using System;
using StillTime.Sts.Commands;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
    public class IncrementVariableNode : SequentialNode {
        public Variable Variable { get; }

        public decimal Increment { get; }

        public IncrementVariableNode(Variable variable, decimal increment) {
            if (variable.Type != StsValueType.Number) {
                throw new Exception("Increment is only valid for number variable.");
            }

            Variable = variable;
            Increment = increment;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            StsValue previousValue = state.GetVariableValue(Variable);
            StsValue newValue = new(previousValue.NumberValue + Increment);
            state.SetVariableValue(Variable, newValue);
        }
    }
}
