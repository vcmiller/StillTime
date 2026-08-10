using System;
using System.Collections.Generic;
using System.Linq;
using StillTime.Game.StateProcessors;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using UnityEngine;

namespace StillTime.Game {
    public class StateAdvancer : MonoBehaviour {
        public List<StateProcessor> _stateProcessors;

        public StateContainer AdvanceState(GameGraph graph, StateContainer currentState, INode nextNode) {
            StateContainer newState = currentState.Clone();

            foreach (StateProcessor stateProcessor in _stateProcessors) {
                stateProcessor.ProcessBeforeAdvance(graph, newState, ref nextNode);
            }

            newState.GetOrCreate<CurrentNodeComponent>().CurrentNode = nextNode;
            newState.GetOrCreate<VisitedNodesComponent>().VisitNode(nextNode, true);
            nextNode.ApplyToState(newState);

            foreach (StateProcessor stateProcessor in _stateProcessors) {
                stateProcessor.ProcessAfterAdvance(graph, newState);
            }

            return newState;
        }
    }
}
