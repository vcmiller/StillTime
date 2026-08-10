using System.Collections.Generic;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class SayNode : TextNode, ISequentialNode {
        public INode Next { get; set; }

        public SayNode(string text, Speaker speaker) : base(text, speaker) { }

        public override IEnumerable<INode> GetPossibleNextNodes(StateContainer state) {
            yield return Next;
        }

        public INode GetSingleNextNode(StateContainer state) {
            return Next;
        }
    }
}
