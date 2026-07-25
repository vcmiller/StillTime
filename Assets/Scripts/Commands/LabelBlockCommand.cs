using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class LabelBlockCommand : Command {
        public string Identifier { get; }

        public List<Command> Commands { get; } = new();

        public LabelBlockCommand(int lineNumber, string line, string identifier) : base(lineNumber, line) {
            Identifier = identifier;
        }

        public override void CreateResources(Dictionary<string, Resource> resources,
                                             Dictionary<string, INode> nodeDictionary) {
            EmptyNode rootNode = new() { FullIdentifier = Identifier };
            nodeDictionary.Add(Identifier, rootNode);
        }
    }
}