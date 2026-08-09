using System.Collections.Generic;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public abstract class SequentialNode : Node, ISequentialNode {
        public INode Next { get; set; }

        public override IEnumerable<INode> GetPossibleNextNodes(StateContainer state) {
            yield return GetSingleNextNode(state);
        }

        public virtual INode GetSingleNextNode(StateContainer state) {
            return Next;
        }
    }
}
