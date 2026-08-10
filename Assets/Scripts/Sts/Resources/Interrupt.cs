using StillTime.Sts.Expressions;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Resources {
    public class Interrupt : Resource {
        public INode TargetNode { get; set; }
        public IExpression Condition { get; set; }
        
        public Interrupt(string identifier) : base(identifier) { }
    }
}