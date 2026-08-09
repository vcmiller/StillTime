using System.Threading;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;
using UnityEngine;

namespace StillTime.Game.NodeViewHandlers {
    public abstract class AnyNodeViewHandler : MonoBehaviour {
        public abstract void HandleState(GameGraph graph, StateContainer state);

        public virtual void HandleInitialState(GameGraph graph, StateContainer state) { }
    }
}
