using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Game;

namespace Nodes {
    public interface INode {
        int Cost { get; set; }

        string FullIdentifier { get; set; }

        public StackTrace CreationStackTrace { get; }

        public string GetSelfIdentifier();

        public IEnumerable<INode> GetPossibleNextNodes(TraversalState state);

        public void ApplyToState(ref MutableTraversalState state);
    }
}
