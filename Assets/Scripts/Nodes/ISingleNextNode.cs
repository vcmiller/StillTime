namespace Nodes {
    public interface ISingleNextNode : INode {
        public INode Next { get; set; }
    }
}