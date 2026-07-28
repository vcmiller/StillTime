namespace Nodes {
    public class SingleTextNode : TextNode, ISingleNextNode {
        public INode Next { get; set; }

        public SingleTextNode(string text, Speaker speaker) : base(text, speaker) {
            // Estimation: 12 chars/sec normal speaking rate. 5 sec default for narration.
            Cost = speaker != null ? text.Length / 12 : 5;
        }
    }
}