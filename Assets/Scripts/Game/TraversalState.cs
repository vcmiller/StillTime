using System.Collections.Generic;
using Nodes;
using UnityEngine;

namespace Game {
    public class TraversalState {
        private readonly HashSet<Gate> _unlockedGates;
        private readonly HashSet<INode> _visitedNodesCurrentRun;
        private readonly HashSet<INode> _visitedNodesOverall;

        public INode CurrentNode { get; }
        
        public INode NodeForTimeout { get; }
        
        public bool ShowCountdown { get; }

        public int? CountdownValue { get; }

        public bool HasReachedUnexploredNode { get; }

        public TraversalState(
            INode node,
            INode nodeForTimeout,
            bool showCountdown,
            int? countdownValue,
            IEnumerable<Gate> unlockedGates,
            IEnumerable<INode> visitedNodesCurrentRun,
            IEnumerable<INode> visitedNodesOverall,
            bool hasReachedUnexploredNode) {
            CurrentNode = node;
            NodeForTimeout = nodeForTimeout;
            ShowCountdown = showCountdown;
            CountdownValue = countdownValue;
            _unlockedGates = new HashSet<Gate>(unlockedGates);
            _visitedNodesCurrentRun = new HashSet<INode>(visitedNodesCurrentRun);
            _visitedNodesOverall = new HashSet<INode>(visitedNodesOverall);
            HasReachedUnexploredNode = hasReachedUnexploredNode;

            HasReachedUnexploredNode |= _visitedNodesCurrentRun.Add(node);

            if (node is UnlockNode unlockNode) {
                _unlockedGates.Add(unlockNode.Gate);
            }
        }

        public IEnumerable<INode> GetAvailableNodes() {
            switch (CurrentNode) {
                case ISingleNextNode singleNextNode:
                    yield return singleNextNode.Next;
                    break;
                case BranchNode branchNode:
                    foreach (Choice choice in branchNode.Choices) {
                        if (IsChoiceAvailable(choice)) {
                            yield return choice.Next;
                        }
                    }

                    break;
            }
        }

        public bool IsChoiceAvailable(Choice choice) {
            if (_visitedNodesCurrentRun.Contains(choice.Next)) return false;
            if (!choice.Gates.TrueForAll(_unlockedGates.Contains)) return false;
            return true;
        }

        public TraversalState Advance(INode next) {
            int? countdownValue = CountdownValue;

            if (ShowCountdown && countdownValue.HasValue) {
                int cost = CurrentNode.Cost;
                while (next is EmptyNode { Next: not null } emptyNode) {
                    cost += next.Cost;
                    next = emptyNode.Next;
                }

                countdownValue = Mathf.Max(0, countdownValue.Value - cost);
            }

            bool showCountdown = ShowCountdown;

            if (next is CountdownNode countdownNode) {
                showCountdown = countdownNode.Show;
                countdownValue = countdownNode.Value ?? countdownValue;
            }

            INode nodeForTimeout = NodeForTimeout;
            if (countdownValue == 0 && ShowCountdown && nodeForTimeout != null) {
                next = nodeForTimeout;
                nodeForTimeout = null;
            }

            TraversalState nextState = new(
                next,
                nodeForTimeout,
                showCountdown,
                countdownValue,
                _unlockedGates,
                _visitedNodesCurrentRun,
                _visitedNodesOverall,
                HasReachedUnexploredNode);

            return nextState;
        }
    }
}