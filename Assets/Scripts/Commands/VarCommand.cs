using System.Collections.Generic;
using Nodes;

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
                "string" => VarType.String,
                _ => throw new ParsingException(lineNumber, line, $"Invalid var type {type}"),
            };
            Name = name;
            Scope = scope switch {
                "run" => VarScope.Run,
                "global" => VarScope.Global,
                _ => throw new ParsingException(lineNumber, line, $"Invalid var scope {scope}"),
            };
        }

        public override void CreateResources(Dictionary<string, Resource> resources,
                                             Dictionary<string, INode> nodeDictionary) {
            Variable variable = new(Name, Type, Scope);
            resources.Add(Name, variable);
        }
    }

    public enum VarType {
        Int,
        Bool,
        String,
    }

    public enum VarScope {
        Run,
        Global,
    }
}