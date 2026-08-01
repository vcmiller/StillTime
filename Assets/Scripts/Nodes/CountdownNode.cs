using StillTime.Game;

namespace StillTime.Nodes {
    public class CountdownNode : SequentialNode {
        public bool Show { get; }

        public int? Value { get; }

        public CountdownNode(bool show, int? value) {
            Show = show;
            Value = value;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.ShowCountdown = Show;
            state.CountdownValue = Value ?? state.CountdownValue;
        }
    }
}
