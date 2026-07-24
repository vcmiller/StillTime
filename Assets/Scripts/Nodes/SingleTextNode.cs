namespace Nodes {
    public class SingleTextNode : TextNode, ISingleNextNode {
        public INode Next { get; set; }
        
        public SingleTextNode(string text, Speaker speaker) : base(text, speaker) { }
    }
}