using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Commands;
using Infohazard.Core;
using Newtonsoft.Json.Linq;
using Nodes;
using UnityEngine;

namespace Game {
    public class GameGraph {
        public INode RootNode { get; }

        public IReadOnlyDictionary<string, INode> NodesByIdentifier { get; }

        public IReadOnlyDictionary<string, Resource> ResourcesByIdentifier { get; }

        public GameGraph(
            INode rootNode,
            IReadOnlyDictionary<string, INode> nodesByIdentifier,
            IReadOnlyDictionary<string, Resource> resourcesByIdentifier) {
            RootNode = rootNode;
            NodesByIdentifier = nodesByIdentifier;
            ResourcesByIdentifier = resourcesByIdentifier;
        }

        public TraversalState BuildInitialState() {
            return new TraversalState(
                RootNode,
                null,
                false,
                null,
                new Dictionary<Variable, object>(),
                new Dictionary<Variable, object>(),
                Enumerable.Empty<INode>(),
                Enumerable.Empty<INode>(),
                Color.black,
                true);
        }

        public SerializedTraversalState SerializeState(TraversalState state) {
            return new SerializedTraversalState {
                RunVariables = state.RunVariables.ToDictionary(v => v.Key.Identifier, v => new JValue(v.Value)),
                GlobalVariables = state.GlobalVariables.ToDictionary(v => v.Key.Identifier, v => new JValue(v.Value)),
                VisitedNodesCurrentRun = state.VisitedNodesCurrentRun.ToList(g => g.FullIdentifier),
                VisitedNodesOverall = state.VisitedNodesOverall.ToList(g => g.FullIdentifier),
                CurrentNode = state.CurrentNode?.FullIdentifier,
                NodeForTimeout = state.NodeForTimeout?.FullIdentifier,
                ShowCountdown = state.ShowCountdown,
                CountdownValue = state.CountdownValue,
                BgColor = ColorUtility.ToHtmlStringRGB(state.BgColor),
                WasCurrentStateUnexplored = state.WasCurrentNodeUnexplored,
            };
        }

        public TraversalState DeserializeState(SerializedTraversalState serializedState) {
            if (!TryGetNode(serializedState.CurrentNode, out INode currentNode)) {
                throw new SerializationException($"Cannot find current node '{serializedState.CurrentNode}'");
            }

            INode nodeForTimeout = null;
            if (!string.IsNullOrWhiteSpace(serializedState.NodeForTimeout)) {
                if (!TryGetNode(serializedState.NodeForTimeout, out nodeForTimeout)) {
                    throw new SerializationException($"Cannot find timeout node '{serializedState.NodeForTimeout}'");
                }
            }

            return new TraversalState(
                currentNode,
                nodeForTimeout,
                serializedState.ShowCountdown,
                serializedState.CountdownValue,
                ConvertVariables(serializedState.RunVariables),
                ConvertVariables(serializedState.GlobalVariables),
                serializedState.VisitedNodesCurrentRun.SelectWhere<string, INode>(TryGetNode),
                serializedState.VisitedNodesOverall.SelectWhere<string, INode>(TryGetNode),
                ColorUtility.TryParseHtmlString($"#{serializedState.BgColor}", out Color bgColor)
                    ? bgColor
                    : Color.black,
                serializedState.WasCurrentStateUnexplored
            );
        }

        private Dictionary<Variable, object> ConvertVariables(Dictionary<string, JValue> values) {
            Dictionary<Variable, object> result = new();

            if (values == null) return result;

            foreach ((string key, JValue jValue) in values) {
                if (!TryGetVariable(key, out Variable variable)) continue;

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

        private bool TryGetVariable(string name, out Variable variable) {
            if (ResourcesByIdentifier.TryGetValue(name, out Resource resource) &&
                resource is Variable temp) {
                variable = temp;
                return true;
            } else {
                variable = null;
                return false;
            }
        }

        private bool TryGetNode(string name, out INode node) {
            if (name == null) {
                node = null;
                return false;
            }

            return NodesByIdentifier.TryGetValue(name, out node);
        }
    }
}
