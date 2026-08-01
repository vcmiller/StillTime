using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
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

        public void Validate() {
            HashSet<INode> seenNodes = new();
            Queue<INode> toExplore = new();
            toExplore.Enqueue(RootNode);

            while (toExplore.TryDequeue(out INode node)) {
                if (!seenNodes.Add(node)) continue;

                if (string.IsNullOrEmpty(node.FullIdentifier)) {
                    StsLibrary.Logger.LogError(
                        "Node {Result} has empty identifier. Creation stack trace:\n{StackTrace}",
                        node,
                        node.CreationStackTrace);
                }
            }
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
                new StsColor(0, 0, 0),
                true);
        }

        public bool TryGetVariable(string name, out Variable variable) {
            if (ResourcesByIdentifier.TryGetValue(name, out Resource resource) &&
                resource is Variable temp) {
                variable = temp;
                return true;
            } else {
                variable = null;
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
