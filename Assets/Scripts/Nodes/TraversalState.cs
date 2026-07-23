using System.Collections.Generic;

namespace Nodes {
    public class TraversalState {
        private readonly HashSet<Gate> _unlockedGates;
        private readonly HashSet<INode> _visitedNodesCurrentRun;
        private readonly HashSet<INode> _visitedNodesOverall;

        public INode CurrentNode { get; }

        public float TimeBudget { get; }

        public bool HasReachedUnexploredNode { get; }

        public TraversalState(
            INode node,
            float timeBudget,
            IEnumerable<Gate> unlockedGates,
            IEnumerable<INode> visitedNodesCurrentRun,
            IEnumerable<INode> visitedNodesOverall,
            bool hasReachedUnexploredNode) {
            CurrentNode = node;
            TimeBudget = timeBudget;
            _unlockedGates = new HashSet<Gate>(unlockedGates);
            _visitedNodesCurrentRun = new HashSet<INode>(visitedNodesCurrentRun);
            _visitedNodesOverall = new HashSet<INode>(visitedNodesOverall);
            HasReachedUnexploredNode = hasReachedUnexploredNode;

            _visitedNodesCurrentRun.Add(node);
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
            float cost = next.Cost;
            while (next is EmptyNode { Next: not null } emptyNode) {
                next = emptyNode.Next;
                cost += next.Cost;
            }

            TraversalState nextState = new(
                next,
                TimeBudget - cost,
                _unlockedGates,
                _visitedNodesCurrentRun,
                _visitedNodesOverall,
                HasReachedUnexploredNode);

            return nextState;
        }
    }
}