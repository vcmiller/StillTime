namespace StillTime.Sts.Nodes {
    public interface ISequentialNode : ISingleNextNode {
        public INode Next { get; set; }
    }
}
