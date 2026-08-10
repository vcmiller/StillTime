using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Runtime.Components {
    public class VisitedNodesComponent : IScopedComponent {
        private readonly Dictionary<Scope, ScopeInfo> _dictionary;

        public VisitedNodesComponent() {
            _dictionary = new Dictionary<Scope, ScopeInfo>();
        }

        private VisitedNodesComponent(Dictionary<Scope, ScopeInfo> dictionary) {
            _dictionary = dictionary;
        }

        public void Initialize(GameGraph gameGraph) {
            foreach (Scope scope in gameGraph.ResourcesByIdentifier.Values.OfType<Scope>()) {
                if (_dictionary.ContainsKey(scope)) continue;
                _dictionary[scope] = new ScopeInfo { VisitedNodes = new HashSet<INode>() };
            }
        }

        public void VisitNode(INode node, bool updateUnexplored) {
            foreach (Scope key in _dictionary.Keys.ToList()) {
                ScopeInfo info = _dictionary[key];
                bool unexplored = info.VisitedNodes.Add(node);
                if (!updateUnexplored) continue;
                info.WasCurrentStateUnexplored = unexplored;
                _dictionary[key] = info;
            }
        }

        public void ResetScope(Scope scope) {
            if (!_dictionary.TryGetValue(scope, out ScopeInfo info)) return;
            info.VisitedNodes.Clear();
        }

        public bool IsVisited(Scope scope, INode node) {
            if (!_dictionary.TryGetValue(scope, out ScopeInfo info)) return false;
            return info.VisitedNodes.Contains(node);
        }

        public bool WasCurrentStateUnexplored(Scope scope) {
            if (!_dictionary.TryGetValue(scope, out ScopeInfo info)) return true;
            return info.WasCurrentStateUnexplored;
        }

        public IStateComponent Clone() {
            return new VisitedNodesComponent(_dictionary.ToDictionary(p => p.Key, p => p.Value.Clone()));
        }

        public JToken Serialize() {
            JObject obj = new();

            foreach ((Scope scope, ScopeInfo info) in _dictionary) {
                JArray nodes = new();
                foreach (INode visitedNode in info.VisitedNodes) {
                    nodes.Add(new JValue(visitedNode.FullIdentifier));
                }

                obj[scope.Identifier] = new JObject {
                    [nameof(ScopeInfo.VisitedNodes)] = nodes,
                    [nameof(ScopeInfo.WasCurrentStateUnexplored)] = info.WasCurrentStateUnexplored,
                };
            }

            return obj;
        }

        public bool Deserialize(GameGraph graph, JToken token) {
            if (token is not JObject obj) return false;
            foreach ((string key, JToken subToken) in obj) {
                if (!graph.TryGetResource(key, out Scope scope) ||
                    !_dictionary.TryGetValue(scope, out ScopeInfo scopeInfo) ||
                    subToken is not JObject subObj ||
                    !subObj.TryGetValue(nameof(ScopeInfo.VisitedNodes), out JToken visitedNodesToken) ||
                    visitedNodesToken is not JArray visitedNodesArray) continue;

                foreach (JToken visitedNodeToken in visitedNodesArray) {
                    if (visitedNodeToken.Type != JTokenType.String) continue;
                    string visitedNodeId = visitedNodeToken.ToObject<string>();
                    if (!graph.TryGetNode(visitedNodeId, out INode node)) continue;
                    scopeInfo.VisitedNodes.Add(node);
                }
            }

            return true;
        }

        private struct ScopeInfo {
            public HashSet<INode> VisitedNodes;
            public bool WasCurrentStateUnexplored;

            public ScopeInfo Clone() {
                return new ScopeInfo {
                    VisitedNodes = new HashSet<INode>(VisitedNodes),
                    WasCurrentStateUnexplored = WasCurrentStateUnexplored,
                };
            }
        }
    }
}
