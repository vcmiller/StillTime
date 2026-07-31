namespace Nodes {
    public interface ISequentialNode : ISingleNextNode {
        public INode Next { get; set; }
    }
}
