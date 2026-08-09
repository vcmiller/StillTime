using System;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using StillTime.Sts.Utility;

namespace StillTime.Game.StateProcessors {
    public class CountdownStateProcessor : StateProcessor {
        public string _countdownVariableName;

        public override void ProcessBeforeAdvance(GameGraph graph, StateContainer state) {
            if (!graph.TryGetResource(_countdownVariableName, out Variable variable)) return;

            VariablesComponent variables = state.GetOrCreate<VariablesComponent>();
            decimal countdownValue = variables.GetVariableValue(variable).NumberValue;

            if (countdownValue <= 0) return;

            decimal cost = GetCost(state.GetOrCreate<CurrentNodeComponent>().CurrentNode);
            decimal newValue = Math.Max(0, countdownValue - cost);
            variables.SetVariableValue(variable, new StsValue(newValue));
        }

        private static decimal GetCost(INode node) {
            return node switch {
                SayNode sayNode => sayNode.Speaker != null ? sayNode.Text.Length / 12 : 5,
                _ => 0,
            };
        }
    }
}
