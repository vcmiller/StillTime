using Game;

namespace Nodes {
    public class DelayNode : SequentialNode {
        public float Time { get; }

        public DelayNode(float time) {
            Time = time;
        }
    }
}
