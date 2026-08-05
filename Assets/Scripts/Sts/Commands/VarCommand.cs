using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Commands {
    public class VarCommand : Command, IResourceCommand {
        public StsValueType Type { get; set; }
        public string Name { get; }
        public string Scope { get; set; }

        public VarCommand(int lineNumber, string line, string type, string name, string scope) :
            base(lineNumber, line) {
            Type = type switch {
                "number" or "num" => StsValueType.Number,
                "color" => StsValueType.Color,
                "bool" => StsValueType.Bool,
                "string" or "str" => StsValueType.String,
                _ => throw new ParsingException(lineNumber, line, $"Invalid var type {type}"),
            };
            Name = name;
            Scope = scope;
        }

        public void CreateResources(Dictionary<string, Resource> resources,
                                    Dictionary<string, INode> nodeDictionary) {
            Variable variable = new(Name, Type, Scope);
            resources.Add(Name, variable);
        }

        public void ValidateResources(Dictionary<string, Resource> resourceDictionary,
                                      Dictionary<string, INode> nodeDictionary) {
            _ = CommandUtility.GetResource<Scope>(this, Scope, resourceDictionary);
        }
    }
}
