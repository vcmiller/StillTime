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

        public override void ApplyAfterAdvanceToSelf(GameGraph graph, StateContainer state) {
            foreach (IScopedComponent scopedComponent in state.Components.Values.OfType<IScopedComponent>()) {
                scopedComponent.ResetScope(Scope);
            }
        }
    }
}
