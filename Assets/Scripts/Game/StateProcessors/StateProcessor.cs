using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;
using UnityEngine;

namespace StillTime.Game.StateProcessors {
    public abstract class StateProcessor : MonoBehaviour {
        public virtual void ProcessBeforeAdvance(GameGraph graph, StateContainer state, ref INode nextNode) { }
        public virtual void ProcessAfterAdvance(GameGraph graph, StateContainer state) { }
    }
}
