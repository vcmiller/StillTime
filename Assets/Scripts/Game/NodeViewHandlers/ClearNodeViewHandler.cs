using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using StillTime.Game.View;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;

namespace StillTime.Game.NodeViewHandlers {
    public class ClearNodeViewHandler : NodeViewHandler<ClearNode> {
        public List<GameViewComponent> _components;

        protected override UniTask<INode> HandleState(
            GameGraph graph,
            StateContainer state,
            ClearNode node,
            CancellationToken cancellationToken) {

            foreach (GameViewComponent component in _components)
                component.Clear();

            return UniTask.FromResult(node.Next);
        }
    }
}
