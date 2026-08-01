using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Infohazard.Core;
using Newtonsoft.Json.Linq;
using StillTime.Sts.Commands;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Game {
    public static class StateSerializer {
        public static SerializedTraversalState SerializeState(TraversalState state) {
            return new SerializedTraversalState {
                RunVariables = state.RunVariables.ToDictionary(v => v.Key.Identifier, v => new JValue(v.Value)),
                GlobalVariables = state.GlobalVariables.ToDictionary(v => v.Key.Identifier, v => new JValue(v.Value)),
                VisitedNodesCurrentRun = state.VisitedNodesCurrentRun.ToList(g => g.FullIdentifier),
                VisitedNodesOverall = state.VisitedNodesOverall.ToList(g => g.FullIdentifier),
                CurrentNode = state.CurrentNode?.FullIdentifier,
                NodeForTimeout = state.NodeForTimeout?.FullIdentifier,
                ShowCountdown = state.ShowCountdown,
                CountdownValue = state.CountdownValue,
                BgColor = state.BgColor.ToHexString(),
                WasCurrentStateUnexplored = state.WasCurrentNodeUnexplored,
            };
        }

        public static TraversalState DeserializeState(GameGraph graph, SerializedTraversalState serializedState) {
            if (!graph.TryGetNode(serializedState.CurrentNode, out INode currentNode)) {
                throw new SerializationException($"Cannot find current node '{serializedState.CurrentNode}'");
            }

            INode nodeForTimeout = null;
            if (!string.IsNullOrWhiteSpace(serializedState.NodeForTimeout)) {
                if (!graph.TryGetNode(serializedState.NodeForTimeout, out nodeForTimeout)) {
                    throw new SerializationException($"Cannot find timeout node '{serializedState.NodeForTimeout}'");
                }
            }

            return new TraversalState(
                currentNode,
                nodeForTimeout,
                serializedState.ShowCountdown,
                serializedState.CountdownValue,
                ConvertVariables(graph, serializedState.RunVariables),
                ConvertVariables(graph, serializedState.GlobalVariables),
                serializedState.VisitedNodesCurrentRun.SelectWhere<string, INode>(graph.TryGetNode),
                serializedState.VisitedNodesOverall.SelectWhere<string, INode>(graph.TryGetNode),
                StsColor.TryParseHex(serializedState.BgColor, out StsColor bgColor)
                    ? bgColor
                    : new StsColor(0, 0, 0),
                serializedState.WasCurrentStateUnexplored
            );
        }

        private static Dictionary<Variable, object> ConvertVariables(GameGraph graph, Dictionary<string, JValue> values) {
            Dictionary<Variable, object> result = new();

            if (values == null) return result;

            foreach ((string key, JValue jValue) in values) {
                if (!graph.TryGetVariable(key, out Variable variable)) continue;

                object value = variable.Type switch {
                    VarType.Int => jValue.ToObject<int>(),
                    VarType.Bool => jValue.ToObject<bool>(),
                    VarType.String => jValue.ToObject<string>(),
                    _ => throw new Exception("Invalid variable type."),
                };

                result[variable] = value;
            }

            return result;
        }
    }
}
