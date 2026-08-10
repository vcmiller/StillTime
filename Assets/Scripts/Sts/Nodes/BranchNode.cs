using System.Collections.Generic;
using System.Linq;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class BranchNode : TextNode {
        public List<IBranchOption> Options { get; } = new();

        public BranchNode(string text, Speaker speaker) : base(text, speaker) { }

        public override IEnumerable<INode> GetPossibleNextNodes(StateContainer state) {
            return Options.Where(o => o.IsAvailable(state)).Select(o => o.GetNextNode(state));
        }
    }
}
