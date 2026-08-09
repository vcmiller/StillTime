using System;
using System.Collections.Generic;
using System.Linq;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Commands.Utility {
    public static class CommandUtility {
        private static readonly List<(string, ComparisonOperator)> ComparisonOps = new() {
            ("==", ComparisonOperator.Equal),
            ("!=", ComparisonOperator.NotEqual),
            ("<=", ComparisonOperator.LessOrEqual),
            (">=", ComparisonOperator.GreaterOrEqual),
            (">", ComparisonOperator.Greater),
            ("<", ComparisonOperator.Less),
        };

        public static ICondition ProcessCondition(Command command, string condition, GraphData graphData) {
            bool invert = condition.StartsWith('!');
            if (invert) condition = condition[1..];

            if (condition.All(c => c == '_' || char.IsLetterOrDigit(c))) {
                Variable variable = graphData.GetResource<Variable>(command, condition);
                return new BoolCondition(variable, !invert);
            }

            foreach ((string str, ComparisonOperator op) in ComparisonOps) {
                int index = condition.IndexOf(str, StringComparison.Ordinal);
                if (index < 0) continue;

                string lhs = condition.AsSpan(0, index).Trim().ToString();
                string rhs = condition.AsSpan(index + str.Length).Trim().ToString();

                Variable variable = graphData.GetResource<Variable>(command, lhs);
                if (variable.Type != StsValueType.Number) {
                    throw new ParsingException(command.LineNumber, command.Line,
                                               $"Cannot use variable of type {variable.Type} as a number operand");
                }

                if (!decimal.TryParse(rhs, out decimal rhsDecimal)) {
                    throw new ParsingException(command.LineNumber, command.Line,
                                               $"Failed to parse value {rhs} as int.");
                }

                return new NumberCondition(variable, op, rhsDecimal, invert);
            }

            throw new ParsingException(command.LineNumber, command.Line, $"Failed to parse condition {condition}");
        }

        public static void AssignIds(string identifierBase, NodeSequenceBuilder builder, GraphData graphData) {
            Dictionary<string, int> countByLocalId = new();
            foreach (INode createdNode in builder.NodeList) {
                createdNode.RegisterStateTypes(graphData.RequiredStates);
                string localId = createdNode.GetSelfIdentifier();
                countByLocalId.TryGetValue(localId, out int count);
                string globalId = $"{identifierBase}{localId}{count}";
                createdNode.FullIdentifier = globalId;
                countByLocalId[localId] = count + 1;
                graphData.Nodes.Add(globalId, createdNode);
            }
        }

        public static void GatherSubCommands<T>(
            ICommand parent,
            ref CommandGatheringState state,
            List<T> results,
            bool errorOnMismatch = true,
            bool stopBeforeEnd = false)
            where T : ICommand {

            bool isFirst = true;
            bool isSingleLine = false;
            while (!state.IsEnded) {
                if (isFirst) isSingleLine = state.Current.LineNumber == parent.LineNumber;
                isFirst = false;

                if (isSingleLine && state.Current.LineNumber > parent.LineNumber) break;
                if (stopBeforeEnd && state.Current is EndCommand) break;
                ICommand command = state.Take();
                if (command is EndCommand) break;

                if (command is not T typedCommand) {
                    if (errorOnMismatch) {
                        throw new ParsingException(
                            state.Current.LineNumber,
                            state.Current.Line,
                            $"Expected command of type {typeof(T).Name} but got {state.Current.GetType().Name}");
                    }

                    break;
                }

                results.Add(typedCommand);
                typedCommand.GatherSubCommands(ref state);
            }
        }

        public static T GatherSubCommand<T>(
            ref CommandGatheringState state,
            bool errorOnMismatch = true,
            bool stopBeforeEnd = false)
            where T : class, ICommand {

            if (state.IsEnded) return null;
            if (stopBeforeEnd && state.Current is EndCommand) return null;
            ICommand command = state.Take();
            if (command is EndCommand) return null;

            if (command is not T typedCommand) {
                if (errorOnMismatch) {
                    throw new ParsingException(
                        state.Current.LineNumber,
                        state.Current.Line,
                        $"Expected command of type {typeof(T).Name} but got {state.Current.GetType().Name}");
                }

                return null;
            }

            typedCommand.GatherSubCommands(ref state);
            return typedCommand;
        }
    }
}
