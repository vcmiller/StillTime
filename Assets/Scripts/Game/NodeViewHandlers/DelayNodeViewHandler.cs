using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;

namespace StillTime.Game.NodeViewHandlers {
    public class DelayNodeViewHandler : NodeViewHandler<DelayNode> {
        protected override async UniTask<INode> HandleState(
            GameGraph graph,
            StateContainer state,
            DelayNode node,
            CancellationToken cancellationToken) {

            await UniTask.Delay(TimeSpan.FromSeconds(node.Time), cancellationToken: cancellationToken);
            return node.Next;
        }
    }
}
