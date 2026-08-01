using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Cysharp.Threading.Tasks;
using StillTime.Sts.Nodes;
using StillTime.Sts.Runtime;
using StillTime.Sts.Utility;
using UnityEngine;

namespace StillTime.Game {
    public class GameRunner : MonoBehaviour {
        public GameView _gameView;
        public bool _alwaysSkip;
        private GameGraph _gameGraph;
        private TraversalState _currentState;
        private CancellationTokenSource _cancellationTokenSource;
        private const string SkipAnimationsKey = "StillTime.SkipAnimations";
        private const string SkipSeenDialogKey = "StillTime.SkipSeenDialog";
        private bool _skipAnimations;
        private bool _skipSeenDialogue;
        private Regex _stringInterpRegex = new(@"\{[0-9a-zA-Z_]*\}");

        public bool SkipAnimations {
            get => _skipAnimations;
            set {
                if (_skipAnimations == value) return;
                _skipAnimations = value;
                PlayerPrefs.SetInt(SkipAnimationsKey, value ? 1 : 0);
            }
        }

        public bool SkipSeenDialogue {
            get => _skipSeenDialogue;
            set {
                if (_skipSeenDialogue == value) return;
                _skipSeenDialogue = value;
                PlayerPrefs.SetInt(SkipSeenDialogKey, value ? 1 : 0);
            }
        }

        private void OnEnable() {
            _skipAnimations = PlayerPrefs.GetInt(SkipAnimationsKey, 0) != 0;
            _skipSeenDialogue = PlayerPrefs.GetInt(SkipSeenDialogKey, 1) != 0;
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

            TraversalState state = StateSerializer.DeserializeState(_gameGraph, data);
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

            return StateSerializer.SerializeState(_currentState);
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
            _gameView.SetBgColor(new StsColor(0, 0, 0), 0);
        }

        private string DoStringInterpolation(string str, TraversalState state) {
            try {
                string currentString = str;
                while (_stringInterpRegex.Match(currentString) is { Success: true } match) {
                    string varName = currentString.Substring(match.Index + 1, match.Length - 2);
                    if (!_gameGraph.ResourcesByIdentifier.TryGetValue(varName, out Resource resource)) {
                        Debug.LogError($"Invalid variable identifier: {varName} in interpolated string {str}");
                        return currentString;
                    }

                    if (resource is not Variable variable) {
                        Debug.LogError($"Invalid variable identifier: {varName} in interpolated string {str}");
                        return currentString;
                    }

                    string varValue = state.GetVariableValue(variable).ToString();

                    currentString = currentString.Replace(match.Value, varValue);
                }

                return currentString;
            } catch (Exception ex) {
                Debug.LogException(ex);
                return str;
            }
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

                bool skipDialog =
                    (_alwaysSkip && Application.isEditor) ||
                    (SkipSeenDialogue && !_currentState.WasCurrentNodeUnexplored);

                switch (_currentState.CurrentNode) {
                    case SayNode singleTextNode:
                        _gameView.SetSingleText(
                            DoStringInterpolation(singleTextNode.Text, _currentState),
                            singleTextNode.Speaker,
                            () => Advance(_currentState, singleTextNode.Next, cancellationToken),
                            SkipAnimations,
                            skipDialog);
                        break;
                    case BranchNode branchNode:
                        _gameView.SetChoices(
                            DoStringInterpolation(branchNode.Text, _currentState),
                            branchNode.Speaker,
                            branchNode.Options
                                      .Where(o => o.IsAvailable(_currentState))
                                      .Select(o => {
                                          INode next = o.GetNextNode(_currentState);
                                          string text = o.GetText(_currentState);
                                          List<TraversalState> stack = new() { _currentState };
                                          TraversalState testState = _currentState.Advance(next);
                                          bool hasNewContent = ExploreBranchForNewContent(stack, testState, 10_000);
                                          return (text,
                                              new Action(() => Advance(_currentState, next, cancellationToken)),
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

                        INode next = singleNextNode.GetSingleNextNode(_currentState);
                        if (next != null) {
                            _currentState = _currentState.Advance(next);
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

        private static bool ExploreBranchForNewContent(List<TraversalState> stack, TraversalState state, int maxDepth) {
            TraversalState previousState = stack[^1];

            if (!previousState.VisitedNodesOverall.Contains(state.CurrentNode)) {
                return true;
            }

            if (stack.Count >= maxDepth) {
                Debug.LogError("Search reached max depth. This should not happen.");
                return true;
            }

            try {
                stack.Add(state);

                foreach (INode possibleNext in state.CurrentNode.GetPossibleNextNodes(state)) {
                    if (possibleNext is null or ResetRunNode) continue;

                    TraversalState previousStateAtNode = stack.FindLast(s => s.CurrentNode == possibleNext);
                    if (previousStateAtNode != null &&
                        previousStateAtNode.GlobalVariables.SequenceEqual(state.GlobalVariables) &&
                        previousStateAtNode.RunVariables.SequenceEqual(state.RunVariables)) {
                        continue;
                    }

                    TraversalState nextState = state.Advance(possibleNext);

                    if (ExploreBranchForNewContent(stack, nextState, maxDepth)) return true;
                }
            } finally {
                if (stack[^1] != state) {
                    throw new Exception("Error in stack operation");
                }

                stack.RemoveAt(stack.Count - 1);
            }

            return false;
        }
    }
}
