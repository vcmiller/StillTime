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

            foreach (Command command in commands) {
                command.CreateResources(resources, nodesByIdentifier);
            }

            foreach (Command command in commands) {
                if (command is not LabelBlockCommand labelBlockCommand) continue;
                string labelId = labelBlockCommand.Identifier;
                EmptyNode labelNode = (EmptyNode)nodesByIdentifier[labelId];
                CommandUtility.ProcessLinearNodes($"{labelId}:", labelNode, labelBlockCommand.Commands, nodesByIdentifier, resources);
            }

            EmptyNode rootNode = new() { FullIdentifier = "#ROOT" };
            nodesByIdentifier[rootNode.FullIdentifier] = rootNode;
            CommandUtility.ProcessLinearNodes(string.Empty, rootNode, commands, nodesByIdentifier, resources);
            return new GameGraph(rootNode, nodesByIdentifier, resources);
        }

    }
}