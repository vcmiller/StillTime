using System.Linq;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class ResetScopeNode : SequentialNode {
        public Scope Scope { get; }

        public ResetScopeNode(Scope scope) {
            Scope = scope;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);

            foreach (Variable variable in state.Variables.Keys.ToList()) {
                if (variable.ScopeId == Scope.Identifier) {
                    state.Variables.Remove(variable);
                }
            }
        }
    }
}
