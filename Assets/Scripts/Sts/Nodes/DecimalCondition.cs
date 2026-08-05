using System;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class DecimalCondition : VariableCondition {
        public ComparisonOperator Operator { get; }

        public decimal ComparisonValue { get; }

        public bool Invert { get; }

        public DecimalCondition(Variable variable, ComparisonOperator op, decimal comparisonValue, bool invert) :
            base(variable) {
            Operator = op;
            ComparisonValue = comparisonValue;
            Invert = invert;
        }

        public override bool CheckCondition(TraversalState traversalState) {
            decimal value = traversalState.GetVariableValue(Variable).NumberValue;

            return Operator switch {
                ComparisonOperator.Equal => value == ComparisonValue,
                ComparisonOperator.NotEqual => value != ComparisonValue,
                ComparisonOperator.Greater => value > ComparisonValue,
                ComparisonOperator.Less => value < ComparisonValue,
                ComparisonOperator.GreaterOrEqual => value >= ComparisonValue,
                ComparisonOperator.LessOrEqual => value <= ComparisonValue,
                _ => throw new ArgumentOutOfRangeException(),
            } ^ Invert;
        }
    }

    public enum ComparisonOperator {
        Equal,
        NotEqual,
        Greater,
        Less,
        GreaterOrEqual,
        LessOrEqual,
    }
}
