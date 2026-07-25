using System;
using Game;

namespace Nodes {
    public class IntCondition : VariableCondition {
        public ComparisonOperator Operator { get; }
        
        public int ComparisonValue { get; }
        
        public IntCondition(Variable variable, ComparisonOperator op, int comparisonValue) : base(variable) {
            Operator = op;
            ComparisonValue = comparisonValue;
        }
        
        public override bool CheckCondition(TraversalState traversalState) {
            int value = traversalState.GetVariableValue<int>(Variable);

            return Operator switch {
                ComparisonOperator.Equal => value == ComparisonValue,
                ComparisonOperator.NotEqual => value != ComparisonValue,
                ComparisonOperator.Greater => value > ComparisonValue,
                ComparisonOperator.Less => value < ComparisonValue,
                ComparisonOperator.GreaterOrEqual => value >= ComparisonValue,
                ComparisonOperator.LessOrEqual => value <= ComparisonValue,
                _ => throw new ArgumentOutOfRangeException(),
            };
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