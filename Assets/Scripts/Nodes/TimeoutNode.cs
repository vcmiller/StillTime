namespace Nodes {
    public class TimeoutNode : Node, ISingleNextNode {
        public INode Next { get; set; }
        
        public INode TimeoutTarget { get; }
        
        public TimeoutNode(INode timeoutTarget) {
            TimeoutTarget = timeoutTarget;
        }
    }
}