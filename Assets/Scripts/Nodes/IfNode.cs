using System.Collections.Generic;
using System.Linq;
using Game;

namespace Nodes {
    public class IfNode : Node, ISingleNextNode {
        public IReadOnlyList<ICondition> Conditions { get; }

        public INode TrueBranch { get; }

        public INode Next { get; set; }

        public IfNode(IReadOnlyList<ICondition> conditions, INode trueBranch) {
            Conditions = conditions;
            TrueBranch = trueBranch;
        }

        public override void ApplyToStateAfterEnd(ref MutableTraversalState state, ref INode nextNode) {
            base.ApplyToStateAfterEnd(ref state, ref nextNode);

            TraversalState testState = new(state, true);
            if (Conditions.All(c => c.CheckCondition(testState))) {
                nextNode = TrueBranch;
            }
        }
    }
}
