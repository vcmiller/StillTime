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

            CurrentNodeComponent currentComponent = newState.GetOrCreate<CurrentNodeComponent>();
            VisitedNodesComponent visitedComponent = newState.GetOrCreate<VisitedNodesComponent>();

            currentComponent.CurrentNode.ApplyBeforeAdvanceFromSelf(graph, newState, ref nextNode);
            currentComponent.CurrentNode = nextNode;
            visitedComponent.VisitNode(nextNode, true);
            nextNode?.ApplyAfterAdvanceToSelf(graph, newState);

            foreach (StateProcessor stateProcessor in _stateProcessors) {
                stateProcessor.ProcessAfterAdvance(graph, newState);
            }

            return newState;
        }
    }
}
