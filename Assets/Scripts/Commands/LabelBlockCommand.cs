using System;
using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class LabelBlockCommand : Command, IResourceCommand, ISubtreeCommand {
        public string Identifier { get; }

        public List<Command> Commands { get; } = new();

        public LabelBlockCommand(int lineNumber, string line, string identifier) : base(lineNumber, line) {
            Identifier = identifier;
        }

        public void CreateResources(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary) {
            EmptyNode rootNode = new() { FullIdentifier = Identifier };
            nodeDictionary.Add(Identifier, rootNode);
        }

        public void BuildNodeTree(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary) {
            if (!nodeDictionary.TryGetValue(Identifier, out INode node) || node is not EmptyNode emptyNode) {
                throw new Exception($"Could not find empty node for label {Identifier} in provided dictionary.");
            }

            CommandUtility.ProcessLinearNodesAndAssignIds($"{Identifier}:", emptyNode, Commands, nodeDictionary, resources, out _);
        }
    }
}
