using System;
using System.Collections.Generic;
using System.Linq;
using Nodes;

namespace Commands {
    public static class CommandUtility {
        private static readonly List<(string, ComparisonOperator)> ComparisonOps = new() {
            ("==", ComparisonOperator.Equal),
            ("!=", ComparisonOperator.NotEqual),
            ("<=", ComparisonOperator.LessOrEqual),
            (">=", ComparisonOperator.GreaterOrEqual),
            (">", ComparisonOperator.Greater),
            ("<", ComparisonOperator.Less),
        };

        public static Speaker GetSpeaker(TextCommand command, IReadOnlyDictionary<string, Resource> resources) {
            if (string.IsNullOrEmpty(command.Speaker)) return null;
            return GetResource<Speaker>(command, command.Speaker, resources);
        }

        public static T GetResource<T>(Command command, string name, IReadOnlyDictionary<string, Resource> resources) {
            if (!resources.TryGetValue(name, out Resource resource)) {
                throw new ParsingException(command.LineNumber, command.Line, $"Invalid resource name {name}");
            }

            if (resource is not T typed) {
                throw new ParsingException(command.LineNumber, command.Line,
                                           $"Resource {name} is wrong type {resource} (expected {typeof(T).Name})");
            }

            return typed;
        }

        public static INode GetNode(Command command, string name, IReadOnlyDictionary<string, INode> nodes) {
            if (!nodes.TryGetValue(name, out INode targetNode)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid target node");
            }

            return targetNode;
        }

        public static ICondition ProcessCondition(Command command, string condition,
                                                  IReadOnlyDictionary<string, Resource> resources) {
            bool invert = condition.StartsWith('!');
            if (invert) condition = condition[1..];

            if (condition.All(c => c == '_' || char.IsLetterOrDigit(c))) {
                Variable variable = GetResource<Variable>(command, condition, resources);
                if (variable.Type != VarType.Bool) {
                    throw new ParsingException(command.LineNumber, command.Line,
                                               $"Cannot use variable of type {variable.Type} as a condition by itself");
                }

                return new BoolCondition(variable, !invert);
            }

            foreach ((string str, ComparisonOperator op) in ComparisonOps) {
                int index = condition.IndexOf(str, StringComparison.Ordinal);
                if (index < 0) continue;

                string lhs = condition.AsSpan(0, index).Trim().ToString();
                string rhs = condition.AsSpan(index + str.Length).Trim().ToString();

                Variable variable = GetResource<Variable>(command, lhs, resources);
                if (variable.Type != VarType.Int) {
                    throw new ParsingException(command.LineNumber, command.Line,
                                               $"Cannot use variable of type {variable.Type} as an int operand");
                }

                if (!int.TryParse(rhs, out int rhsInt)) {
                    throw new ParsingException(command.LineNumber, command.Line,
                                               $"Failed to parse value {rhs} as int.");
                }

                return new IntCondition(variable, op, rhsInt, invert);
            }

            throw new ParsingException(command.LineNumber, command.Line, $"Failed to parse condition {condition}");
        }

        public static void ProcessLinearNodesAndAssignIds(
            string identifierBase,
            ISequentialNode previousNode,
            List<Command> commands,
            Dictionary<string, INode> nodesByIdentifier,
            Dictionary<string, Resource> resources,
            out ISequentialNode lastNode) {

            List<INode> createdNodes = new();

            ProcessLinearNodes(previousNode, commands, nodesByIdentifier, resources, createdNodes, out lastNode);

            Dictionary<string, int> countByLocalId = new();
            foreach (INode createdNode in createdNodes) {
                string localId = createdNode.GetSelfIdentifier();
                countByLocalId.TryGetValue(localId, out int count);
                string globalId = $"{identifierBase}{localId}{count}";
                createdNode.FullIdentifier = globalId;
                countByLocalId[localId] = count + 1;
                nodesByIdentifier.Add(globalId, createdNode);
            }
        }

        public static void ProcessLinearNodes(
            ISequentialNode previousNode,
            List<Command> commands,
            Dictionary<string, INode> nodesByIdentifier,
            Dictionary<string, Resource> resources,
            List<INode> createdNodes,
            out ISequentialNode lastNode) {

            foreach (Command command in commands) {
                if (previousNode == null) break;

                if (command is not ISequentialCommand sequentialCommand) continue;
                sequentialCommand.ApplyToSequence(ref previousNode, resources, nodesByIdentifier, createdNodes);
            }

            lastNode = previousNode;
        }
    }
}
