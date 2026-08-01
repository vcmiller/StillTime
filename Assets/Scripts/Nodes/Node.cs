using System.Collections.Generic;
using System.Diagnostics;
using Game;

namespace Nodes {
    public abstract class Node : INode {
        public int Cost { get; set; }

        public string FullIdentifier { get; set; }

        public StackTrace CreationStackTrace { get; }

        protected Node() {
#if UNITY_EDITOR
            CreationStackTrace = new StackTrace(2);
#endif
        }

        public virtual string GetSelfIdentifier() {
            return GetType().Name;
        }

        public abstract IEnumerable<INode> GetPossibleNextNodes(TraversalState state);

        public virtual void ApplyToState(ref MutableTraversalState state) { }
    }
}
