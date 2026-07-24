using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Nodes;
using UnityEngine;

namespace Game {
    public class NodeRunner : MonoBehaviour {
        public GameView _gameView;

        public void RunNode(TraversalState state) {
            HashSet<INode> seenNodes = new();
            while (true) {
                if (!seenNodes.Add(state.CurrentNode)) {
                    throw new Exception(
                        $"Encountered a node {state.CurrentNode} twice in the same synchronous call to RunNode. " +
                        "This could easily result in an infinite loop.");
                }
                
                if (!isActiveAndEnabled) return;
                
                CheckTimer(state);

                switch (state.CurrentNode) {
                    case SingleTextNode singleTextNode:
                        _gameView.SetSingleText(
                            singleTextNode.Text, 
                            singleTextNode.Speaker, 
                            () => Advance(state, singleTextNode.Next));
                        break;
                    case BranchNode branchNode:
                        _gameView.SetChoices(
                            branchNode.Text,
                            branchNode.Speaker,
                            branchNode.Choices
                                      .Where(state.IsChoiceAvailable)
                                      .Select(c => {
                                          TraversalState nextState = state.Advance(c.Next);
                                          bool hasNewContent = ExploreBranchForNewContent(nextState, 0, 10000);
                                          return (c.Text, new Action(() => Advance(state, c.Next)), hasNewContent);
                                      })
                                      .ToList());
                        break;
                    case DelayNode delayNode:
                        UniTask.Delay(TimeSpan.FromSeconds(delayNode.Time))
                               .ContinueWith(() => Advance(state, delayNode.Next));
                        break;
                    case ISingleNextNode singleNextNode:
                        switch (singleNextNode) {
                            case BgNode bgNode:
                                _gameView.SetBgColor(bgNode.Color, bgNode.Time);
                                break;
                            case ClearNode:
                                _gameView.Clear(true);
                                break;
                        }
                        
                        if (singleNextNode.Next != null) {
                            state = state.Advance(singleNextNode.Next);
                            continue;
                        } else {
                            _gameView.Clear(true);
                            break;
                        }
                }

                break;
            }
        }

        private void CheckTimer(TraversalState state) {
            if (state.ShowCountdown && state.CountdownValue != null) {
                _gameView.ShowCountdown(TimeSpan.FromSeconds(state.CountdownValue.Value).ToString("c"));
            } else if (!state.ShowCountdown) {
                _gameView.HideCountdown();
            }
        }

        private void Advance(TraversalState state, INode next) {
            if (next == null) {
                _gameView.Clear(true);
            } else {
                RunNode(state.Advance(next));
            }
        }

        private bool ExploreBranchForNewContent(TraversalState state, int depth, int maxDepth) {
            if (state.WasSelfNodeUnexplored) return true;
            
            if (depth == maxDepth) {
                Debug.LogError("Search reached max depth. This should not happen.");
                return true;
            }
            
            foreach (INode possibleNext in state.GetAvailableNodes()) {
                if (possibleNext == null) continue;
                
                TraversalState nextState = state.Advance(possibleNext);
                if (ExploreBranchForNewContent(nextState, depth + 1, maxDepth)) return true;
            }

            return false;
        }
    }
}