using System.Linq;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;

namespace StillTime.Sts.Nodes {
    public class ResetScopeNode : SequentialNode {
        public Scope Scope { get; }

        public ResetScopeNode(Scope scope) {
            Scope = scope;
        }

        public override void ApplyToState(StateContainer state) {
            base.ApplyToState(state);

            foreach (IScopedComponent scopedComponent in state.Components.Values.OfType<IScopedComponent>()) {
                scopedComponent.ResetScope(Scope);
            }
        }
    }
}
