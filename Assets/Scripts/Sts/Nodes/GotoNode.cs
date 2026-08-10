using System.Collections.Generic;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class GotoNode : Node, ISingleNextNode {
        public INode Target { get; }
        
        public GotoNode(INode target) {
            Target = target;
        }
        
        public override IEnumerable<INode> GetPossibleNextNodes(StateContainer state) {
            yield return Target;
        }
        
        public INode GetSingleNextNode(StateContainer state) {
            return Target;
        }
    }
}