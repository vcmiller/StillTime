using System.Collections.Generic;
using System.Linq;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class IfNode : Node, ISingleNextNode {
        public IReadOnlyList<ICondition> Conditions { get; }

        public INode TrueBranch { get; set; }

        public INode FalseBranch { get; set; }

        public IfNode(IReadOnlyList<ICondition> conditions) {
            Conditions = conditions;
        }

        public INode GetSingleNextNode(StateContainer state) {
            if (Conditions.All(c => c.CheckCondition(state))) {
                return TrueBranch;
            } else {
                return FalseBranch;
            }
        }

        public override IEnumerable<INode> GetPossibleNextNodes(StateContainer state) {
            yield return GetSingleNextNode(state);
        }
    }
}
