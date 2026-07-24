using System.Collections.Generic;
using Commands;
using Game;

namespace Nodes {
    public static class GraphBuilder {
        public static GameGraph BuildGraph(List<Command> commands) {
            Dictionary<string, Resource> resources = new();
            Dictionary<string, INode> nodesByIdentifier = new();

            // Build gates and labels first.
            foreach (Command command in commands) {
                if (command is GateCommand gateCommand) {
                    Gate gate = new(gateCommand.GateName);
                    resources.TryAdd(gate.Identifier, gate);
                }

                if (command is SpeakerCommand speakerCommand) {
                    Speaker speaker = new(speakerCommand.Name, speakerCommand.Color, speakerCommand.Text);
                    resources.Add(speakerCommand.Name, speaker);
                }

                if (command is LabelBlockCommand labelBlockCommand) {
                    EmptyNode labelRoot = new() { FullIdentifier = labelBlockCommand.Identifier };
                    nodesByIdentifier.Add(labelRoot.FullIdentifier, labelRoot);
                }
            }

            // Initialize the label blocks.
            foreach (Command command in commands) {
                if (command is not LabelBlockCommand labelBlockCommand) continue;
                string labelId = labelBlockCommand.Identifier;
                EmptyNode labelNode = (EmptyNode)nodesByIdentifier[labelId];
                ProcessLinearNodes($"{labelId}:", labelNode, labelBlockCommand.Commands, nodesByIdentifier, resources);
            }

            EmptyNode rootNode = new();
            ProcessLinearNodes(string.Empty, rootNode, commands, nodesByIdentifier, resources);
            return new GameGraph(rootNode, nodesByIdentifier, resources);
        }

        private static Speaker GetSpeaker(TextCommand command, Dictionary<string, Resource> resources) {
            if (string.IsNullOrEmpty(command.Speaker)) return null;
            return GetResource<Speaker>(command, command.Speaker, resources);
        }

        private static T GetResource<T>(Command command, string name, Dictionary<string, Resource> resources) {
            if (!resources.TryGetValue(name, out Resource resource)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid speaker name");
            }

            if (resource is not T typed) {
                throw new ParsingException(command.LineNumber, command.Line,
                    $"Resource {name} is wrong type {resource} (expected {typeof(T).Name})");
            }

            return typed;
        }

        private static INode GetNode(Command command, string name, Dictionary<string, INode> nodes) {
            if (!nodes.TryGetValue(name, out INode targetNode)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid target node");
            }

            return targetNode;
        }

        private static void ProcessLinearNodes(
            string identifierBase,
            ISingleNextNode previousNode,
            List<Command> commands,
            Dictionary<string, INode> nodesByIdentifier,
            Dictionary<string, Resource> resources) {

            Dictionary<string, int> countByLocalId = new();
            
            foreach (Command command in commands) {
                if (previousNode == null) break;
                INode createdNode = null;
                
                Speaker speaker = command is TextCommand tc ? GetSpeaker(tc, resources) : null;
                switch (command) {
                    case BranchBlockCommand branchBlockCommand:
                        BranchNode branchNode = new(branchBlockCommand.Text, speaker);
                        foreach (ChoiceCommand choiceCommand in branchBlockCommand.Choices) {
                            branchNode.Choices.Add(ProcessChoice(choiceCommand, nodesByIdentifier, resources));
                        }

                        createdNode = branchNode;
                        previousNode.Next = branchNode;
                        previousNode = null;
                        break;
                    case GotoCommand gotoCommand:
                        INode gotoTarget = GetNode(gotoCommand, gotoCommand.TargetLabel, nodesByIdentifier);

                        if (gotoCommand.ResetRunState) {
                            ResetRunNode resetNode = new();
                            previousNode.Next = resetNode;
                            previousNode = resetNode;
                        }

                        previousNode.Next = gotoTarget;
                        previousNode = null;
                        break;
                    case CostCommand costCommand:
                        previousNode.Cost += costCommand.Cost;
                        break;
                    case TimeoutCommand timeoutCommand:
                        INode timeoutTarget = GetNode(timeoutCommand, timeoutCommand.TargetLabel, nodesByIdentifier);
                        TimeoutNode timeoutNode = new(timeoutTarget);
                        previousNode.Next = timeoutNode;
                        previousNode = timeoutNode;
                        createdNode = timeoutNode;
                        break;
                    case SayCommand sayCommand:
                        SingleTextNode sayNode = new(sayCommand.Text, speaker);
                        previousNode.Next = sayNode;
                        previousNode = sayNode;
                        createdNode = sayNode;
                        break;
                    case UnlockCommand unlockCommand:
                        Gate gate = GetResource<Gate>(unlockCommand, unlockCommand.GateName, resources);
                        UnlockNode unlockNode = new(gate);
                        previousNode.Next = unlockNode;
                        previousNode = unlockNode;
                        createdNode = unlockNode;
                        break;
                    case CountdownCommand countdownCommand:
                        CountdownNode countdownNode = new(countdownCommand.Show, countdownCommand.Value);
                        previousNode.Next = countdownNode;
                        previousNode = countdownNode;
                        break;
                    case BgCommand bgCommand:
                        BgNode bgNode = new(bgCommand.Color, bgCommand.Time);
                        previousNode.Next = bgNode;
                        previousNode = bgNode;
                        createdNode = bgNode;
                        break;
                    case DelayCommand delayCommand:
                        DelayNode delayNode = new(delayCommand.Time);
                        previousNode.Next = delayNode;
                        previousNode = delayNode;
                        createdNode = delayNode;
                        break;
                    case ClearCommand:
                        ClearNode clearNode = new();
                        previousNode.Next = clearNode;
                        previousNode = clearNode;
                        createdNode = clearNode;
                        break;
                    default:
                        continue;
                }

                if (createdNode != null) {
                    string localId = createdNode.GetSelfIdentifier();
                    countByLocalId.TryGetValue(localId, out int count);
                    string globalId = $"{identifierBase}{localId}{count}";
                    createdNode.FullIdentifier = globalId;
                    countByLocalId[localId] = count + 1;
                    nodesByIdentifier.Add(globalId, createdNode);
                }
            }
        }

        private static Choice ProcessChoice(
            ChoiceCommand command,
            Dictionary<string, INode> nodesByIdentifier,
            Dictionary<string, Resource> resources) {

            if (!nodesByIdentifier.TryGetValue(command.TargetLabel, out INode choiceTarget)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid target label");
            }

            Choice choice = new(command.Text, choiceTarget, command.AlwaysAllow);
            foreach (string gateName in command.RequiredGates) {
                if (!resources.TryGetValue(gateName, out Resource resource)) {
                    throw new ParsingException(command.LineNumber, command.Line, "Invalid gate name");
                }

                if (resource is not Gate gate) {
                    throw new ParsingException(command.LineNumber, command.Line, $"Resource {gateName} is wrong type {resource}");
                }

                choice.Gates.Add(gate);
            }

            return choice;
        }
    }
}