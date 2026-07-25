using Commands;

namespace Nodes {
    public class Variable : Resource {
        public VarType Type { get; }
        
        public VarScope Scope { get; }

        public object DefaultValue => Type switch {
            VarType.Int => 0,
            VarType.Bool => false,
            VarType.String => string.Empty,
        };
        
        public Variable(string identifier, VarType type, VarScope scope) : base(identifier) {
            Type = type;
            Scope = scope;
        }
    }
}