using System.Threading;
using Cysharp.Threading.Tasks;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;

namespace StillTime.Game.NodeViewHandlers {
    public class ClearNodeViewHandler : NodeViewHandler<ClearNode> {
        public GameViewRoot _viewRoot;

        protected override UniTask<INode> HandleState(
            GameGraph graph,
            StateContainer state,
            ClearNode node,
            CancellationToken cancellationToken) {

            _viewRoot.Clear();

            return UniTask.FromResult(node.Next);
        }
    }
}
