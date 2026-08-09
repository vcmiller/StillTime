using Newtonsoft.Json.Linq;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Runtime.Components {
    public class CurrentNodeComponent : IStateComponent {
        public INode CurrentNode { get; set; }

        public IStateComponent Clone() {
            return new CurrentNodeComponent { CurrentNode = CurrentNode };
        }

        public JToken Serialize() {
            return new JValue(CurrentNode?.FullIdentifier);
        }

        public bool Deserialize(GameGraph graph, JToken token) {
            if (token.Type != JTokenType.String) return false;

            string id = token.ToObject<string>();
            if (!graph.TryGetNode(id, out INode node)) return false;
            CurrentNode = node;
            return true;
        }
    }
}
