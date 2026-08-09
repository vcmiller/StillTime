using System.Threading;
using Cysharp.Threading.Tasks;
using StillTime.Game.View;
using StillTime.Sts.Commands;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;

namespace StillTime.Game.NodeViewHandlers {
    public class BgNodeViewHandler : NodeViewHandler<BgNode> {
        public BackgroundView _view;
        public GameSettings _gameSettings;

        protected override UniTask<INode> HandleState(
            GameGraph graph,
            StateContainer state,
            BgNode node,
            CancellationToken cancellationToken) {

            _view.SetColor(node.Color, _gameSettings.SkipAnimations ? 0 : node.Time);
            return UniTask.FromResult(node.Next);
        }

        public override void HandleInitialState(GameGraph graph, StateContainer state) {
            if (!graph.TryGetResource(BgCommand.BuiltInVariableName, out Variable variable)) return;

            _view.SetColor(state.GetOrCreate<VariablesComponent>().GetVariableValue(variable).ColorValue, 0);
        }
    }
}
