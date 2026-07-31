using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Game;

namespace Nodes {
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
            nodesByIdentifier[rootNode.FullIdentifier] = rootNode;
            CommandUtility.ProcessLinearNodes(string.Empty, rootNode, commands, nodesByIdentifier, resources);
            return new GameGraph(rootNode, nodesByIdentifier, resources);
        }

    }
}
