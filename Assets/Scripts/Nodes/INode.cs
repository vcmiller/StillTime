using System.Collections.Generic;
using System.Linq;
using Game;

namespace Nodes {
    public interface INode {
        int Cost { get; set; }

        string FullIdentifier { get; set; }

        public string GetSelfIdentifier();

        public IEnumerable<INode> GetPossibleNextNodes(TraversalState state);

        public void ApplyToState(ref MutableTraversalState state);
    }
}
