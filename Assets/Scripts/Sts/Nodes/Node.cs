using System;
using System.Collections.Generic;
using System.Diagnostics;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public abstract class Node : INode {
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

        public abstract IEnumerable<INode> GetPossibleNextNodes(StateContainer state);

        public virtual void ApplyToState(StateContainer state) { }

        public virtual void RegisterStateTypes(HashSet<Type> stateTypes) { }
    }
}
