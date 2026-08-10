using System.Collections.Generic;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;

namespace StillTime.Sts.Nodes {
    public class PushNode : SequentialNode{
        public INode Target { get; }

        public PushNode(INode target) {
            Target = target;
        }

        public override IEnumerable<INode> GetPossibleNextNodes(StateContainer state) {
            yield return Target;
        }

        public override INode GetSingleNextNode(StateContainer state) {
            return Target;
        }

        public override void ApplyBeforeAdvanceFromSelf(GameGraph graph, StateContainer state, ref INode nextNode) {
            CurrentNodeComponent component = state.GetOrCreate<CurrentNodeComponent>();
            component.NodeStack.Add(Next);
        }
    }
}
