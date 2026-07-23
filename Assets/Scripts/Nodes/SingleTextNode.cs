namespace Nodes {
    public class SingleTextNode : TextNode, ISingleNextNode {
        public INode Next { get; set; }
    }
}