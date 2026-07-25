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
    }
}