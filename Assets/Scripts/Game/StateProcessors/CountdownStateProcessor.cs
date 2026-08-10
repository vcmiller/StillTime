using System;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using StillTime.Sts.Utility;

namespace StillTime.Game.StateProcessors {
    public class CountdownStateProcessor : StateProcessor {
        public string _countdownVariableName;

        public override void ProcessBeforeAdvance(GameGraph graph, StateContainer state, ref INode nextNode) {
            if (!graph.TryGetResource(_countdownVariableName, out Variable variable)) return;

            VariablesComponent variables = state.GetOrCreate<VariablesComponent>();
            decimal countdownValue = variables.GetVariableValue(variable).NumberValue;

            if (countdownValue <= 0) return;

            int cost = GetCost(state.GetOrCreate<CurrentNodeComponent>().CurrentNode);
            decimal newValue = Math.Max(0, countdownValue - cost);
            variables.SetVariableValue(variable, new StsValue(newValue));
        }

        private static int GetCost(INode node) {
            return node switch {
                TextNode textNode => textNode.Speaker != null ? textNode.Text.Length / 12 : 5,
                _ => 0,
            };
        }
    }
}
