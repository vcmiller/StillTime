using System.Collections.Generic;
using System.Linq;
using StillTime.Game;

namespace StillTime.Nodes {
    public class IfNode : Node, ISingleNextNode {
        public IReadOnlyList<ICondition> Conditions { get; }

        public INode TrueBranch { get; set; }

        public INode FalseBranch { get; set; }

        public IfNode(IReadOnlyList<ICondition> conditions) {
            Conditions = conditions;
        }

        public INode GetSingleNextNode(TraversalState state) {
            if (Conditions.All(c => c.CheckCondition(state))) {
                return TrueBranch;
            } else {
                return FalseBranch;
            }
        }

        public override IEnumerable<INode> GetPossibleNextNodes(TraversalState state) {
            yield return GetSingleNextNode(state);
        }
    }
}
