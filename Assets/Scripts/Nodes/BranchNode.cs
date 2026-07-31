using System.Collections.Generic;

namespace Nodes {
    public class BranchNode : TextNode {
        public List<IBranchOption> Options { get; } = new();

        public BranchNode(string text, Speaker speaker) : base(text, speaker) {
            // Estimation: 12 chars/sec normal speaking rate.
            Cost = speaker != null ? text.Length / 12 : 0;
        }
    }
}
