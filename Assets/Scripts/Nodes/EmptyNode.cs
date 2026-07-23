namespace Nodes {
    public class EmptyNode : Node, ISingleNextNode {
        public INode Next { get; set; }
    }
}