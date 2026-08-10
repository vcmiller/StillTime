using System;
using Infohazard.StillTimeScript.Core.Nodes;
using Infohazard.StillTimeScript.Core.Resource;
using Infohazard.StillTimeScript.Core.State;
using Infohazard.StillTimeScript.Game.View.Components;
using Infohazard.StillTimeScript.Game.View.Handlers;

namespace StillTime.Game.View {
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
