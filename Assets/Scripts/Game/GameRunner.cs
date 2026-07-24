using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Nodes;
using UnityEngine;

namespace Game {
    public class GameRunner : MonoBehaviour {
        public GameView _gameView;
        private GameGraph _gameGraph;
        private TraversalState _currentState;
        private CancellationTokenSource _cancellationTokenSource;

        public void LoadGameGraph(GameGraph gameGraph) {
            _gameGraph = gameGraph;
        }

        public void StartNewGame() {
            if (_gameGraph == null) {
                throw new InvalidOperationException("Game graph must be loaded before starting game.");
            }

            if (_currentState != null) {
                throw new InvalidOperationException("Game is already running.");
            }

            TraversalState newState = new(
                _gameGraph.RootNode,
                null,
                false,
                null,
                Enumerable.Empty<Gate>(),
                Enumerable.Empty<INode>(),
                Enumerable.Empty<INode>(),
                true,
                Color.black);

            _cancellationTokenSource = new CancellationTokenSource();
            RunNode(newState, _cancellationTokenSource.Token);
        }

        public void LoadGame(SerializedTraversalState data) {
            if (_gameGraph == null) {
                throw new InvalidOperationException("Game graph must be loaded before starting game.");
            }

            if (_currentState != null) {
                throw new InvalidOperationException("Game is already running.");
            }

            TraversalState state = _gameGraph.DeserializeState(data);
            _cancellationTokenSource = new CancellationTokenSource();
            RunNode(state, _cancellationTokenSource.Token);
        }

        public SerializedTraversalState SaveGame() {
            if (_gameGraph == null) {
                throw new InvalidOperationException("Game graph must be loaded before starting game.");
            }

            if (_currentState == null) {
                throw new InvalidOperationException("Game is not running.");
            }

            return _gameGraph.SerializeState(_currentState);
        }

        public void ClearGameState() {
            if (_gameGraph == null) {
                throw new InvalidOperationException("Game graph must be loaded before starting game.");
            }

            _gameView.Clear(true);
            _currentState = null;
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private void RunNode(TraversalState state, CancellationToken cancellationToken) {
            _currentState = state;
            HashSet<INode> seenNodes = new();
            while (!cancellationToken.IsCancellationRequested) {
                if (!seenNodes.Add(_currentState.CurrentNode)) {
                    throw new Exception(
                        $"Encountered a node {_currentState.CurrentNode} twice in the same synchronous call to RunNode. " +
                        "This could easily result in an infinite loop.");
                }

                if (!isActiveAndEnabled) return;

                CheckTimer(_currentState);

                switch (_currentState.CurrentNode) {
                    case SingleTextNode singleTextNode:
                        _gameView.SetSingleText(
                            singleTextNode.Text,
                            singleTextNode.Speaker,
                            () => Advance(_currentState, singleTextNode.Next, cancellationToken));
                        break;
                    case BranchNode branchNode:
                        _gameView.SetChoices(
                            branchNode.Text,
                            branchNode.Speaker,
                            branchNode.Choices
                                      .Where(_currentState.IsChoiceAvailable)
                                      .Select(c => {
                                          TraversalState nextState = _currentState.Advance(c.Next);
                                          bool hasNewContent = ExploreBranchForNewContent(nextState, 0, 10000);
                                          return (c.Text, new Action(() => Advance(_currentState, c.Next, cancellationToken)),
                                              hasNewContent);
                                      })
                                      .ToList());
                        break;
                    case DelayNode delayNode:
                        UniTask.Delay(TimeSpan.FromSeconds(delayNode.Time), cancellationToken: cancellationToken)
                               .ContinueWith(() => Advance(_currentState, delayNode.Next, cancellationToken));
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
                            _currentState = _currentState.Advance(singleNextNode.Next);
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

        private void Advance(TraversalState state, INode next, CancellationToken cancellationToken) {
            if (cancellationToken.IsCancellationRequested) return;
            
            if (next == null) {
                _gameView.Clear(true);
            } else {
                RunNode(state.Advance(next), cancellationToken);
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