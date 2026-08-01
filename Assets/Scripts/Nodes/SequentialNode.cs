using System.Collections.Generic;
using StillTime.Game;

namespace StillTime.Nodes {
    public class SequentialNode : Node, ISequentialNode {
        public INode Next { get; set; }

        public override IEnumerable<INode> GetPossibleNextNodes(TraversalState state) {
            yield return GetSingleNextNode(state);
        }

        public virtual INode GetSingleNextNode(TraversalState state) {
            return Next;
        }
    }
}
