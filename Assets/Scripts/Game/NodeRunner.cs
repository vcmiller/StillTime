using System;
using System.Linq;
using Nodes;
using UnityEngine;

namespace Game {
    public class NodeRunner : MonoBehaviour {
        public GameView _gameView;

        public void RunNode(TraversalState state) {
            while (true) {
                if (!isActiveAndEnabled) return;

                switch (state.CurrentNode) {
                    case SingleTextNode singleTextNode:
                        _gameView.SetSingleText(singleTextNode.Text, () => Advance(state, singleTextNode.Next));
                        break;
                    case BranchNode branchNode:
                        _gameView.SetChoices(branchNode.Text,
                            branchNode.Choices
                                      .Where(state.IsChoiceAvailable)
                                      .Select(c => (c.Text, new Action(() => Advance(state, c.Next))))
                                      .ToList());
                        break;
                    case ISingleNextNode singleNextNode:
                        if (singleNextNode.Next != null) {
                            state = state.Advance(singleNextNode.Next);
                            continue;
                        } else {
                            _gameView.Clear();
                            break;
                        }
                }

                break;
            }
        }

        private void Advance(TraversalState state, INode next) {
            if (next == null) {
                _gameView.Clear();
            } else {
                RunNode(state.Advance(next));
            }
        }
    }
}