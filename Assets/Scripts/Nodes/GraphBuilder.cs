using System;
using System.Collections.Generic;
using System.Linq;
using Commands;

namespace Nodes {
    public static class GraphBuilder {
        public static INode BuildGraph(List<Command> commands, out INode nodeForTimeout) {
            Dictionary<string, EmptyNode> labels = new();
            Dictionary<string, Gate> gates = new();
            Dictionary<string, Speaker> speakers = new();

            // Build gates and labels first.
            foreach (Command command in commands) {
                if (command is GateCommand gateCommand) {
                    gates.TryAdd(gateCommand.GateName, new Gate { Identifier = gateCommand.GateName });
                }

                if (command is LabelBlockCommand labelBlockCommand) {
                    labels.Add(labelBlockCommand.Identifier, new EmptyNode());
                }

                if (command is SpeakerCommand speakerCommand) {
                    speakers.Add(speakerCommand.Name,
                        new Speaker(speakerCommand.Name, speakerCommand.Color, speakerCommand.Text));
                }
            }

            // Initialize the label blocks.
            foreach (Command command in commands) {
                if (command is not LabelBlockCommand labelBlockCommand) continue;
                EmptyNode labelNode = labels[labelBlockCommand.Identifier];
                ProcessLinearNodes(labelNode, labelBlockCommand.Commands, labels, gates, speakers);
            }
            
            // Check for timeout command.
            TimeoutCommand timeoutCommand = null;
            foreach (Command command in commands) {
                if (command is not TimeoutCommand temp) continue;

                if (timeoutCommand == null) {
                    timeoutCommand = temp;
                } else {
                    throw new ParsingException(command.LineNumber, command.Line, "Can only have a single timeout command");
                }
            }

            if (timeoutCommand != null) {
                if (labels.TryGetValue(timeoutCommand.Target, out EmptyNode label)) {
                    nodeForTimeout = label;
                } else {
                    throw new ParsingException(timeoutCommand.LineNumber, timeoutCommand.Line, 
                        "Invalid target label");
                }
            } else {
                nodeForTimeout = null;
            }

            EmptyNode rootNode = new();
            ProcessLinearNodes(rootNode, commands, labels, gates, speakers);
            return rootNode;
        }

        private static Speaker GetSpeaker(TextCommand command, Dictionary<string, Speaker> speakers) {
            if (string.IsNullOrEmpty(command.Speaker)) return null;
            
            if (!speakers.TryGetValue(command.Speaker, out Speaker speaker)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid speaker name");
            }

            return speaker;
        }

        private static void ProcessLinearNodes(
            ISingleNextNode previousNode,
            List<Command> commands,
            Dictionary<string, EmptyNode> labelNodes,
            Dictionary<string, Gate> gates,
            Dictionary<string, Speaker> speakers) {
            
            foreach (Command command in commands) {
                Speaker speaker = command is TextCommand tc ? GetSpeaker(tc, speakers) : null;
                switch (command) {
                    case CostCommand costCommand:
                        previousNode.Cost += costCommand.Cost;
                        break;
                    case BranchBlockCommand branchBlockCommand:
                        BranchNode branchNode = new(branchBlockCommand.Text, speaker);
                        foreach (ChoiceCommand choiceCommand in branchBlockCommand.Choices) {
                            branchNode.Choices.Add(ProcessChoice(choiceCommand, labelNodes, gates));
                        }

                        previousNode.Next = branchNode;
                        return;
                    case GotoCommand gotoCommand:
                        if (!labelNodes.TryGetValue(gotoCommand.TargetLabel, out EmptyNode targetNode)) {
                            throw new ParsingException(gotoCommand.LineNumber, gotoCommand.Line, 
                                "Invalid target label");
                        }

                        if (gotoCommand.ResetRunState) {
                            ResetRunNode resetNode = new();
                            previousNode.Next = resetNode;
                            previousNode = resetNode;
                        }

                        previousNode.Next = targetNode;
                        return;
                    case SayCommand sayCommand:
                        SingleTextNode sayNode = new(sayCommand.Text, speaker);
                        previousNode.Next = sayNode;
                        previousNode = sayNode;
                        break;
                    case UnlockCommand unlockCommand:
                        UnlockNode unlockNode = new();
                        if (!gates.TryGetValue(unlockCommand.GateName, out Gate gate)) {
                            throw new ParsingException(unlockCommand.LineNumber, unlockCommand.Line, 
                                "Invalid gate name");
                        }

                        unlockNode.Gate = gate;
                        previousNode.Next = unlockNode;
                        previousNode = unlockNode;
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
                        break;
                    case DelayCommand delayCommand:
                        DelayNode delayNode = new(delayCommand.Time);
                        previousNode.Next = delayNode;
                        previousNode = delayNode;
                        break;
                    case ClearCommand:
                        ClearNode clearNode = new();
                        previousNode.Next = clearNode;
                        previousNode = clearNode;
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

            if (!labelNodes.TryGetValue(command.TargetLabel, out EmptyNode choiceTarget)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid target label");
            }

            Choice choice = new(command.Text, choiceTarget, command.AlwaysAllow);
            foreach (string gateName in command.RequiredGates) {
                if (!gates.TryGetValue(gateName, out Gate gate)) {
                    throw new ParsingException(command.LineNumber, command.Line, "Invalid gate name");
                }

                choice.Gates.Add(gate);
            }

            return choice;
        }
    }
}