using System;
using System.Collections.Generic;
using System.Diagnostics;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public interface INode {
        int Cost { get; set; }

        string FullIdentifier { get; set; }

        public StackTrace CreationStackTrace { get; }

        public string GetSelfIdentifier();

        public IEnumerable<INode> GetPossibleNextNodes(StateContainer state);

        public void ApplyToState(StateContainer state);

        public void RegisterStateTypes(HashSet<Type> stateTypes) { }
    }
}
