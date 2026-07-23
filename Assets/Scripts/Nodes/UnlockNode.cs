namespace Nodes {
    public class UnlockNode : Node, ISingleNextNode {
        public INode Next { get; set; }
    
        public Gate Gate { get; set; }
    }
}