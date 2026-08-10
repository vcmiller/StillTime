using System;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Expressions {
    public class UnaryMathExpression : IExpression {
        public StsValueType Type => StsValueType.Number;
        public UnaryMathOperator Operator { get; }
        public IExpression SubExpression { get; }
        
        public UnaryMathExpression(UnaryMathOperator op, IExpression subExpression) {
            if (subExpression.Type != StsValueType.Number) {
                throw new ArgumentException("Subexpression must be a number");
            }
            
            Operator = op;
            SubExpression = subExpression;
        }

        public StsValue Evaluate(StateContainer state) {
            StsValue subResult = SubExpression.Evaluate(state);
            return Operator switch {
                UnaryMathOperator.Negate => new StsValue(-subResult.NumberValue),
                _ => throw new Exception("Unknown unary math operator"),
            };
        }
    }
    
    public enum UnaryMathOperator {
        Negate,
    }
}