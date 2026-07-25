using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Infohazard.Core;
using Nodes;
using UnityEngine;

namespace Game {
    public class GameRunner : MonoBehaviour {
        public GameView _gameView;
        private GameGraph _gameGraph;
        private TraversalState _currentState;
        private CancellationTokenSource _cancellationTokenSource;
        private const string SkipAnimationsKey = "StillTime.SkipAnimations";
        private bool _skipAnimations;

        public bool SkipAnimations {
            get => _skipAnimations;
            set {
                if (_skipAnimations == value) return;
                _skipAnimations = value;
                PlayerPrefs.SetInt(SkipAnimationsKey, value ? 1 : 0);
            }
        }

        private void OnEnable() {
            _skipAnimations = PlayerPrefs.GetInt(SkipAnimationsKey) != 0;
        }

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

            TraversalState state = _gameGraph.BuildInitialState();
            _cancellationTokenSource = new CancellationTokenSource();
            _gameView.SetBgColor(state.BgColor, 0);
            RunNode(state, _cancellationTokenSource.Token);
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
            _gameView.SetBgColor(state.BgColor, 0);
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
            _gameView.SetBgColor(Color.black, 0);
        }

        private void RunNode(TraversalState state, CancellationToken cancellationToken) {
            _currentState = state;
            HashSet<INode> seenNodes = new();
            while (!cancellationToken.IsCancellationRequested) {
                if (!seenNodes.Add(_currentState.CurrentNode)) {
                    throw new Exception(
                        $"Encountered a node {_currentState.CurrentNode.FullIdentifier} twice in the same synchronous call to RunNode. " +
                        "This could easily result in an infinite loop.");
                }

                if (!isActiveAndEnabled) return;

                CheckTimer(_currentState);

                switch (_currentState.CurrentNode) {
                    case SingleTextNode singleTextNode:
                        _gameView.SetSingleText(
                            singleTextNode.Text,
                            singleTextNode.Speaker,
                            () => Advance(_currentState, singleTextNode.Next, cancellationToken),
                            SkipAnimations);
                        break;
                    case BranchNode branchNode:
                        _gameView.SetChoices(
                            branchNode.Text,
                            branchNode.Speaker,
                            branchNode.Choices
                                      .Where(_currentState.IsChoiceAvailable)
                                      .Select(c => {
                                          Stack<TraversalState> stack = new();
                                          stack.Push(_currentState.Advance(c.Next));
                                          bool hasNewContent = ExploreBranchForNewContent(stack, 10000);
                                          return (c.Text,
                                              new Action(() => Advance(_currentState, c.Next, cancellationToken)),
                                              hasNewContent);
                                      })
                                      .ToList(),
                            SkipAnimations);
                        break;
                    case DelayNode delayNode when !SkipAnimations:
                        UniTask.Delay(TimeSpan.FromSeconds(delayNode.Time), cancellationToken: cancellationToken)
                               .ContinueWith(() => Advance(_currentState, delayNode.Next, cancellationToken));
                        break;
                    case ISingleNextNode singleNextNode:
                        switch (singleNextNode) {
                            case BgNode bgNode:
                                _gameView.SetBgColor(bgNode.Color, SkipAnimations ? 0 : bgNode.Time);
                                break;
                            case ClearNode:
                                _gameView.Clear(true);
                                break;
                        }

                        if (singleNextNode.Next != null) {
                            _currentState = _currentState.Advance(singleNextNode.Next);
                            Debug.Log($"Going to next state '{_currentState.CurrentNode.FullIdentifier}'");
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

        private static bool ExploreBranchForNewContent(Stack<TraversalState> stack, int maxDepth) {
            if (!stack.TryPeek(out TraversalState state)) return false;

            if (state.WasSelfNodeUnexplored) return true;

            if (stack.Count >= maxDepth) {
                Debug.LogError("Search reached max depth. This should not happen.");
                return true;
            }

            foreach (INode possibleNext in state.GetAvailableNodes()) {
                if (possibleNext is null or ResetRunNode) continue;

                TraversalState previousState = stack.LastOrDefault(s => s.CurrentNode == possibleNext);
                if (previousState != null && 
                    previousState.GlobalVariables.SequenceEqual(state.GlobalVariables) &&
                    previousState.RunVariables.SequenceEqual(state.RunVariables)) {
                    continue;
                }

                TraversalState nextState = state.Advance(possibleNext);

                stack.Push(nextState);

                try {
                    if (ExploreBranchForNewContent(stack, maxDepth)) return true;
                } finally {
                    TraversalState poppedState = stack.Pop();
                    if (poppedState != nextState) {
                        throw new Exception("Error in stack operation");
                    }
                }
            }

            return false;
        }
    }
}