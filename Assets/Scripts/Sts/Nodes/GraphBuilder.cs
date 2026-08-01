using System.Collections.Generic;
using System.Linq;
using StillTime.Sts.Commands;

namespace StillTime.Sts.Nodes {
    public static class GraphBuilder {

        public static GameGraph BuildGraph(List<Command> commands) {
            Dictionary<string, Resource> resources = new();
            Dictionary<string, INode> nodesByIdentifier = new();

            foreach (IResourceCommand resourceCommand in commands.OfType<IResourceCommand>()) {
                resourceCommand.CreateResources(resources, nodesByIdentifier);
            }

            foreach (ISubtreeCommand command in commands.OfType<ISubtreeCommand>()) {
                command.BuildNodeTree(resources, nodesByIdentifier);
            }

            EmptyNode rootNode = new() { FullIdentifier = "#ROOT" };
            ISequentialNode currentNode = rootNode;
            nodesByIdentifier[rootNode.FullIdentifier] = rootNode;
            CommandUtility.ProcessLinearNodesAndAssignIds(string.Empty, ref currentNode, commands, nodesByIdentifier,
                                                          resources);
            return new GameGraph(rootNode, nodesByIdentifier, resources);
        }

    }
}
