namespace Nodes {
    public class DelayNode : Node, ISingleNextNode {
        public INode Next { get; set; }
        
        public float Time { get; }

        public DelayNode(float time) {
            Time = time;
        }
    }
}