using System.Collections.Generic;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class SayNode : TextNode, ISequentialNode {
        public INode Next { get; set; }

        public SayNode(string text, Speaker speaker) : base(text, speaker) {
            // Estimation: 12 chars/sec normal speaking rate. 5 sec default for narration.
            Cost = speaker != null ? text.Length / 12 : 5;
        }

        public override IEnumerable<INode> GetPossibleNextNodes(TraversalState state) {
            yield return Next;
        }

        public INode GetSingleNextNode(TraversalState state) {
            return Next;
        }
    }
}
