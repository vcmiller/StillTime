using System.Collections.Generic;
using StillTime.Sts.Expressions;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class IfNode : Node, ISingleNextNode {
        public IExpression Condition { get; }

        public INode TrueBranch { get; set; }

        public INode FalseBranch { get; set; }

        public IfNode(IExpression condition) {
            Condition = condition;
        }

        public INode GetSingleNextNode(StateContainer state) {
            if (Condition.Evaluate(state).ToBool()) {
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
