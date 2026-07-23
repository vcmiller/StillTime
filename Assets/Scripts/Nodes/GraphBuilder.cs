using System;
using System.Collections.Generic;
using Commands;

namespace Nodes {
    public static class GraphBuilder {
        public static INode BuildGraph(List<Command> commands) {
            Dictionary<string, EmptyNode> labels = new();
            Dictionary<string, Gate> gates = new();

            // Build gates and labels first.
            foreach (Command command in commands) {
                if (command is GateCommand gateCommand) {
                    gates.TryAdd(gateCommand.Name, new Gate { Identifier = gateCommand.Name });
                }

                if (command is LabelBlockCommand labelBlockCommand) {
                    labels.Add(labelBlockCommand.Identifier, new EmptyNode());
                }
            }

            // Initialize the label blocks.
            foreach (Command command in commands) {
                if (command is not LabelBlockCommand labelBlockCommand) continue;
                EmptyNode labelNode = labels[labelBlockCommand.Identifier];
                ProcessLinearNodes(labelNode, labelBlockCommand.Commands, labels, gates);
            }

            EmptyNode rootNode = new();
            ProcessLinearNodes(rootNode, commands, labels, gates);
            return rootNode;
        }

        private static void ProcessLinearNodes(
            ISingleNextNode previousNode,
            List<Command> commands,
            Dictionary<string, EmptyNode> labelNodes,
            Dictionary<string, Gate> gates) {
            
            foreach (Command command in commands) {
                switch (command) {
                    case CostCommand costCommand:
                        previousNode.Cost += costCommand.Cost;
                        break;
                    case BranchBlockCommand branchBlockCommand:
                        BranchNode branchNode = new() { Text = branchBlockCommand.Text };
                        foreach (ChoiceCommand choiceCommand in branchBlockCommand.Choices) {
                            branchNode.Choices.Add(ProcessChoice(choiceCommand, labelNodes, gates));
                        }

                        previousNode.Next = branchNode;
                        return;
                    case GotoCommand gotoCommand:
                        if (!labelNodes.TryGetValue(gotoCommand.TargetLabel, out EmptyNode targetNode)) {
                            throw new Exception($"Invalid goto target {gotoCommand.TargetLabel}");
                        }

                        previousNode.Next = targetNode;
                        return;
                    case SayCommand sayCommand:
                        SingleTextNode sayNode = new() { Text = sayCommand.Text };
                        previousNode.Next = sayNode;
                        previousNode = sayNode;
                        break;
                    case UnlockCommand unlockCommand:
                        UnlockNode unlockNode = new();
                        if (!gates.TryGetValue(unlockCommand.Gate, out Gate gate)) {
                            throw new Exception($"Invalid gate name {unlockCommand.Gate}.");
                        }

                        unlockNode.Gate = gate;
                        previousNode.Next = unlockNode;
                        previousNode = unlockNode;
                        break;
                    default:
                        continue;
                }
            }
        }

        private static Choice ProcessChoice(
            ChoiceCommand command,
            Dictionary<string, EmptyNode> labelNodes,
            Dictionary<string, Gate> gates) {
            
            Choice choice = new() { Text = command.Text };
            foreach (string gateName in command.RequiredGates) {
                if (!gates.TryGetValue(gateName, out Gate gate)) {
                    throw new Exception($"Invalid gate name {gateName}.");
                }

                choice.Gates.Add(gate);
            }

            if (!labelNodes.TryGetValue(command.TargetLabel, out EmptyNode choiceTarget)) {
                throw new Exception($"Invalid choice target {command.TargetLabel}");
            }

            choice.Next = choiceTarget;
            return choice;
        }
    }
}