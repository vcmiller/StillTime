using System;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Expressions {
    public class BinaryLogicExpression : IExpression {
        public StsValueType Type => StsValueType.Bool;

        public BinaryLogicOperator Operator { get; }
        
        public IExpression Left { get; }
        public IExpression Right { get; }
        
        public BinaryLogicExpression(IExpression left, IExpression right, BinaryLogicOperator op) {
            Left = left;
            Right = right;
            Operator = op;
        }
        
        public StsValue Evaluate(StateContainer state) {
            bool result = Operator switch {
                BinaryLogicOperator.And => Left.Evaluate(state).ToBool() && Right.Evaluate(state).ToBool(),
                BinaryLogicOperator.Or => Left.Evaluate(state).ToBool() || Right.Evaluate(state).ToBool(),
                BinaryLogicOperator.Xor => Left.Evaluate(state).ToBool() ^ Right.Evaluate(state).ToBool(),
                _ => throw new ArgumentOutOfRangeException(),
            };
            
            return new StsValue(result);
        }
    }

    public enum BinaryLogicOperator {
        And,
        Or,
        Xor,
    }
}