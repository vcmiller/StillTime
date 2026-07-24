using System.Collections.Generic;
using System.Linq;
using Mono.Collections.Generic;
using Nodes;
using UnityEngine;
using Utility;

namespace Game {
    public class TraversalState {
        public ReadOnlySet<Gate> UnlockedGates { get; }
        
        public ReadOnlySet<INode> VisitedNodesCurrentRun { get; }
        
        public ReadOnlySet<INode> VisitedNodesOverall { get; }

        public INode CurrentNode { get; }
        
        public INode NodeForTimeout { get; }
        
        public bool ShowCountdown { get; }

        public int? CountdownValue { get; }
        
        public bool WasSelfNodeUnexplored { get; }
        
        public Color BgColor { get; }

        public TraversalState(
            INode node,
            INode nodeForTimeout,
            bool showCountdown,
            int? countdownValue,
            IEnumerable<Gate> unlockedGates,
            IEnumerable<INode> visitedNodesCurrentRun,
            IEnumerable<INode> visitedNodesOverall,
            bool wasSelfNodeUnexplored,
            Color bgColor) {
            CurrentNode = node;
            NodeForTimeout = nodeForTimeout;
            ShowCountdown = showCountdown;
            CountdownValue = countdownValue;
            UnlockedGates = new ReadOnlySet<Gate>(unlockedGates);
            VisitedNodesCurrentRun = new ReadOnlySet<INode>(visitedNodesCurrentRun);
            VisitedNodesOverall = new ReadOnlySet<INode>(visitedNodesOverall);
            WasSelfNodeUnexplored = wasSelfNodeUnexplored;
            BgColor = bgColor;
        }

        private TraversalState(
            INode node,
            INode nodeForTimeout,
            bool showCountdown,
            int? countdownValue,
            ReadOnlySet<Gate> unlockedGates,
            ReadOnlySet<INode> visitedNodesCurrentRun,
            ReadOnlySet<INode> visitedNodesOverall,
            bool wasSelfNodeUnexplored,
            Color bgColor) {
            CurrentNode = node;
            NodeForTimeout = nodeForTimeout;
            ShowCountdown = showCountdown;
            CountdownValue = countdownValue;
            UnlockedGates = unlockedGates;
            VisitedNodesCurrentRun = visitedNodesCurrentRun;
            VisitedNodesOverall = visitedNodesOverall;
            WasSelfNodeUnexplored = wasSelfNodeUnexplored;
            BgColor = bgColor;
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
            if (!choice.AlwaysAllow && VisitedNodesCurrentRun.Contains(choice.Next)) return false;
            if (!choice.Gates.TrueForAll(UnlockedGates.Contains)) return false;
            return true;
        }

        public TraversalState Advance(INode next) {
            int? countdownValue = CountdownValue;
            bool showCountdown = ShowCountdown;
            HashSet<Gate> unlockedSet = new(UnlockedGates);
            HashSet<INode> visitedSetCurrentRun = new(VisitedNodesCurrentRun);
            HashSet<INode> visitedSetOverall = new(VisitedNodesOverall);
            Color bgColor = BgColor;

            visitedSetCurrentRun.Add(next);

            if (showCountdown && countdownValue.HasValue) {
                countdownValue = Mathf.Max(0, countdownValue.Value - CurrentNode.Cost);
            }

            INode nodeForTimeout = NodeForTimeout;
            if (countdownValue == 0 && showCountdown && nodeForTimeout != null) {
                next = nodeForTimeout;
                nodeForTimeout = null;
            }

            switch (next) {
                case CountdownNode countdownNode:
                    showCountdown = countdownNode.Show;
                    countdownValue = countdownNode.Value ?? countdownValue;
                    break;
                case ResetRunNode:
                    showCountdown = false;
                    countdownValue = null;
                    visitedSetCurrentRun.Clear();
                    nodeForTimeout = null;
                    break;
                case TimeoutNode timeoutNode:
                    nodeForTimeout = timeoutNode.TimeoutTarget;
                    break;
                case BgNode bgNode:
                    bgColor = bgNode.Color;
                    break;
                case UnlockNode unlockNode:
                    unlockedSet.Add(unlockNode.Gate);
                    break;
            }
            
            bool wasSelfNodeUnexplored = visitedSetOverall.Add(next);
            
            TraversalState nextState = new(
                next,
                nodeForTimeout,
                showCountdown,
                countdownValue,
                new ReadOnlySet<Gate>(unlockedSet),
                new ReadOnlySet<INode>(visitedSetCurrentRun),
                new ReadOnlySet<INode>(visitedSetOverall),
                wasSelfNodeUnexplored,
                bgColor);

            return nextState;
        }
    }
}