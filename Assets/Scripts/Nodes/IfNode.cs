using System.Collections.Generic;
using System.Linq;
using Game;

namespace Nodes {
    public class IfNode : SequentialNode {
        public IReadOnlyList<ICondition> Conditions { get; }

        public INode TrueBranch { get; }

        public IfNode(IReadOnlyList<ICondition> conditions, INode trueBranch) {
            Conditions = conditions;
            TrueBranch = trueBranch;
        }

        public override INode GetSingleNextNode(TraversalState state) {
            if (Conditions.All(c => c.CheckCondition(state))) {
                return TrueBranch;
            } else {
                return Next;
            }
        }
    }
}
