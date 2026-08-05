using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Infohazard.Core;
using Newtonsoft.Json.Linq;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Game {
    public static class StateSerializer {
        public static SerializedTraversalState SerializeState(TraversalState state) {
            return new SerializedTraversalState {
                Variables = state.Variables.ToDictionary(v => v.Key.Identifier, v => new JValue(v.Value)),
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

            Dictionary<Variable, StsValue> variables = new();
            ConvertVariables(graph, serializedState.GlobalVariables, variables);
            ConvertVariables(graph, serializedState.RunVariables, variables);
            ConvertVariables(graph, serializedState.Variables, variables);

            return new TraversalState(
                currentNode,
                nodeForTimeout,
                serializedState.ShowCountdown,
                serializedState.CountdownValue,
                variables,
                serializedState.VisitedNodesCurrentRun.SelectWhere<string, INode>(graph.TryGetNode),
                serializedState.VisitedNodesOverall.SelectWhere<string, INode>(graph.TryGetNode),
                StsColor.TryParseHex(serializedState.BgColor, out StsColor bgColor)
                    ? bgColor
                    : new StsColor(0, 0, 0),
                serializedState.WasCurrentStateUnexplored
            );
        }

        private static void ConvertVariables(
            GameGraph graph,
            Dictionary<string, JValue> src,
            Dictionary<Variable, StsValue> dest) {

            if (src == null) return;

            foreach ((string key, JValue jValue) in src) {
                if (!graph.TryGetVariable(key, out Variable variable)) continue;

                StsValue value = variable.Type switch {
                    StsValueType.Number => new StsValue(jValue.ToObject<decimal>()),
                    StsValueType.Color => new StsValue(
                        StsColor.TryParseHex(jValue.ToObject<string>(), out StsColor color) ? color : default),
                    StsValueType.Bool => new StsValue(jValue.ToObject<bool>()),
                    StsValueType.String => new StsValue(jValue.ToObject<string>()),
                    StsValueType.None => default,
                    _ => throw new Exception("Invalid variable type."),
                };

                dest[variable] = value;
            }
        }
    }
}
