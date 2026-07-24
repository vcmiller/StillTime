namespace Nodes {
    public class CountdownNode : Node, ISingleNextNode {
        public INode Next { get; set; }
        
        public bool Show { get; }
        
        public int? Value { get; }

        public CountdownNode(bool show, int? value) {
            Show = show;
            Value = value;
        }
    }
}