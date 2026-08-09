using Newtonsoft.Json.Linq;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Runtime.Components {
    public interface IStateComponent {
        public IStateComponent Clone();

        public void Initialize(GameGraph graph) { }

        public JToken Serialize();

        public bool Deserialize(GameGraph graph, JToken token);
    }
}
