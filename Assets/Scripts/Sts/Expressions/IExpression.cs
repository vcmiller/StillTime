using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Expressions {
    public interface IExpression {
        public StsValueType Type { get; }
        
        public StsValue Evaluate(StateContainer state);
    }
}