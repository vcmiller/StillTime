using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Infohazard.Core;
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
                Enumerable.Empty<Gate>(),
                Enumerable.Empty<INode>(),
                Enumerable.Empty<INode>(),
                false,
                Color.black);
        }

        public SerializedTraversalState SerializeState(TraversalState state) {
            return new SerializedTraversalState {
                UnlockedGates = state.UnlockedGates.ToList(g => g.Identifier),
                VisitedNodesCurrentRun = state.VisitedNodesCurrentRun.ToList(g => g.FullIdentifier),
                VisitedNodesOverall = state.VisitedNodesOverall.ToList(g => g.FullIdentifier),
                CurrentNode = state.CurrentNode?.FullIdentifier,
                NodeForTimeout = state.NodeForTimeout?.FullIdentifier,
                ShowCountdown = state.ShowCountdown,
                CountdownValue = state.CountdownValue,
                WasSelfNodeUnexplored = state.WasSelfNodeUnexplored,
                BgColor = ColorUtility.ToHtmlStringRGB(state.BgColor),
            };
        }

        public TraversalState DeserializeState(SerializedTraversalState serializedState) {
            if (!TryGetNode(serializedState.CurrentNode, out INode currentNode)) {
                throw new SerializationException($"Cannot find current node {serializedState.CurrentNode}");
            }

            INode nodeForTimeout = null;
            if (!string.IsNullOrWhiteSpace(serializedState.NodeForTimeout)) {
                if (!TryGetNode(serializedState.NodeForTimeout, out nodeForTimeout)) {
                    throw new SerializationException($"Cannot find timeout node {serializedState.NodeForTimeout}");
                }
            }

            return new TraversalState(
                currentNode,
                nodeForTimeout,
                serializedState.ShowCountdown,
                serializedState.CountdownValue,
                serializedState.UnlockedGates.SelectWhere<string, Gate>(TryGetGate),
                serializedState.VisitedNodesCurrentRun.SelectWhere<string, INode>(TryGetNode),
                serializedState.VisitedNodesOverall.SelectWhere<string, INode>(TryGetNode),
                serializedState.WasSelfNodeUnexplored,
                ColorUtility.TryParseHtmlString($"#{serializedState.BgColor}", out Color bgColor)
                    ? bgColor
                    : Color.black);
        }

        private bool TryGetGate(string name, out Gate gate) {
            if (ResourcesByIdentifier.TryGetValue(name, out Resource resource) &&
                resource is Gate temp) {
                gate = temp;
                return true;
            } else {
                gate = null;
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