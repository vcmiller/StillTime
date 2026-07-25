namespace Commands {
    public class VarCommand : Command {
        public VarType Type { get; set; }
        public string Name { get; }
        public VarScope Scope { get; set; }

        public VarCommand(int lineNumber, string line, string type, string name, string scope) :
            base(lineNumber, line) {
            Type = type switch {
                "int" => VarType.Int,
                "bool" => VarType.Bool,
                "String" => VarType.String,
                _ => throw new ParsingException(lineNumber, line, $"Invalid var type {type}"),
            };
            Name = name;
            Scope = scope switch {
                "run" => VarScope.Run,
                "global" => VarScope.Global,
                _ => throw new ParsingException(lineNumber, line, $"Invalid var scope {scope}"),
            };
        }
    }

    public enum VarType {
        Int, Bool, String,
    }

    public enum VarScope {
        Run, Global,
    }
}