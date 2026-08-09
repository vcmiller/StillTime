using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
    public class BgNode : SequentialNode {
        public StsColor Color { get; }

        public float Time { get; }

        public Variable Variable { get; }

        public BgNode(StsColor color, float time, Variable variable) {
            Color = color;
            Time = time;
            Variable = variable;
        }

        public override void ApplyToState(StateContainer state) {
            base.ApplyToState(state);
            VariablesComponent component = state.GetOrCreate<VariablesComponent>();
            component.SetVariableValue(Variable, new StsValue(Color));
        }
    }
}
