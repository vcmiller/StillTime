using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
    public class GameGraph {
        public INode RootNode { get; }

        public IReadOnlyDictionary<string, INode> NodesByIdentifier { get; }

        public IReadOnlyDictionary<string, Resource> ResourcesByIdentifier { get; }

        public IReadOnlyList<Type> StateComponentTypes { get; }

        public GameGraph(
            INode rootNode,
            IReadOnlyDictionary<string, INode> nodesByIdentifier,
            IReadOnlyDictionary<string, Resource> resourcesByIdentifier,
            List<Type> stateComponentTypes) {
            RootNode = rootNode;
            NodesByIdentifier = nodesByIdentifier;
            ResourcesByIdentifier = resourcesByIdentifier;
            StateComponentTypes = stateComponentTypes;
        }

        public void Validate() {
            HashSet<INode> seenNodes = new();
            Queue<INode> toExplore = new();
            toExplore.Enqueue(RootNode);

            while (toExplore.TryDequeue(out INode node)) {
                if (!seenNodes.Add(node)) continue;

                if (string.IsNullOrEmpty(node.FullIdentifier)) {
                    StsLibrary.LogError(
                        $"Node {node} has empty identifier. Creation stack trace:\n{node.CreationStackTrace}");
                }
            }
        }

        public StateContainer BuildEmptyState() {
            StateContainer container = new();

            foreach (Type type in StateComponentTypes) {
                IStateComponent component = (IStateComponent)Activator.CreateInstance(type);
                container.Set(type, component);
            }

            return container;
        }

        public bool TryGetResource<T>(string name, out T result) where T : Resource {
            if (ResourcesByIdentifier.TryGetValue(name, out Resource resource) && resource is T temp) {
                result = temp;
                return true;
            } else {
                result = null;
                return false;
            }
        }

        public bool TryGetNode(string name, out INode node) {
            if (name == null) {
                node = null;
                return false;
            }

            return NodesByIdentifier.TryGetValue(name, out node);
        }
    }
}
