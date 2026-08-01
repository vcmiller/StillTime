using System.Collections.Generic;
using System.Linq;
using StillTime.Game;

namespace StillTime.Nodes {
    public class BranchNode : TextNode {
        public List<IBranchOption> Options { get; } = new();

        public BranchNode(string text, Speaker speaker) : base(text, speaker) {
            // Estimation: 12 chars/sec normal speaking rate.
            Cost = speaker != null ? text.Length / 12 : 0;
        }

        public override IEnumerable<INode> GetPossibleNextNodes(TraversalState state) {
            return Options.Where(o => o.IsAvailable(state)).Select(o => o.GetNextNode(state));
        }
    }
}
