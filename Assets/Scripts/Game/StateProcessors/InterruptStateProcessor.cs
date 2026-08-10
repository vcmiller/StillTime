using System.Linq;
using System.Threading;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;

namespace StillTime.Game.StateProcessors {
    public class InterruptStateProcessor : StateProcessor {
        public override void ProcessBeforeAdvance(GameGraph graph, StateContainer state, ref INode nextNode) {
            foreach (Interrupt interrupt in graph.ResourcesByIdentifier.Values.OfType<Interrupt>()) {
                if (!interrupt.Condition.Evaluate(state).ToBool()) continue;

                nextNode = interrupt.TargetNode;
                return;
            }
        }
    }
}