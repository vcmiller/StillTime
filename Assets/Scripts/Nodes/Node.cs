using System.Collections.Generic;
using Game;

namespace Nodes {
    public abstract class Node : INode {
        public int Cost { get; set; }

        public string FullIdentifier { get; set; }

        public virtual string GetSelfIdentifier() {
            return GetType().Name;
        }

        public abstract IEnumerable<INode> GetPossibleNextNodes(TraversalState state);

        public virtual void ApplyToState(ref MutableTraversalState state) { }
    }
}
