using System;
using System.Collections.Generic;
using System.Diagnostics;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public interface INode {
        string FullIdentifier { get; set; }

        public StackTrace CreationStackTrace { get; }

        public string GetSelfIdentifier();

        public IEnumerable<INode> GetPossibleNextNodes(StateContainer state);

        public void ApplyAfterAdvanceToSelf(GameGraph graph, StateContainer state);

        public void ApplyBeforeAdvanceFromSelf(GameGraph graph, StateContainer state, ref INode nextNode);

        public void RegisterStateTypes(HashSet<Type> stateTypes) { }
    }
}
