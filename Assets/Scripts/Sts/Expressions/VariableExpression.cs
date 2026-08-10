using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Expressions {
    public class VariableExpression : IExpression {
        public StsValueType Type => Variable.Type;
        
        public Variable Variable { get; }
        
        public VariableExpression(Variable variable) {
            Variable = variable;
        }
        
        public StsValue Evaluate(StateContainer state) {
            return state.GetOrCreate<VariablesComponent>().GetVariableValue(Variable);
        }
    }
}