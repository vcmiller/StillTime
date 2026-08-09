using System;
using StillTime.Game.View;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;

namespace StillTime.Game.NodeViewHandlers {
    public class CountdownNodeViewHandler : AnyNodeViewHandler {
        public CountdownView _view;
        public string _countdownVariableName;

        public override void HandleState(GameGraph graph, StateContainer state) {
            if (!graph.TryGetResource(_countdownVariableName, out Variable variable)) return;

            decimal countdownValue = state.GetOrCreate<VariablesComponent>().GetVariableValue(variable).NumberValue;

            if (countdownValue >= 0) {
                _view.ShowCountdown(TimeSpan.FromSeconds((double)countdownValue).ToString("c"));
            } else {
                _view.HideCountdown();
            }
        }
    }
}
