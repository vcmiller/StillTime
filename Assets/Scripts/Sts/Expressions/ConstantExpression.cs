using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Expressions {
    public class ConstantExpression : IExpression {
        public StsValueType Type => Value.ValueType;
        
        public StsValue Value { get; }

        public ConstantExpression(StsValue value) {
            Value = value;
        }
        
        public StsValue Evaluate(StateContainer state) => Value;
    }
}