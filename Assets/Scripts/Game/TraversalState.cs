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

        private TraversalState(MutableTraversalState mutableState) {
            CurrentNode = mutableState.CurrentNode;
            NodeForTimeout = mutableState.NodeForTimeout;
            ShowCountdown = mutableState.ShowCountdown;
            CountdownValue = mutableState.CountdownValue;
            UnlockedGates = new ReadOnlySet<Gate>(mutableState.UnlockedGates);
            VisitedNodesCurrentRun = new ReadOnlySet<INode>(mutableState.VisitedNodesCurrentRun);
            VisitedNodesOverall = new ReadOnlySet<INode>(mutableState.VisitedNodesOverall);
            WasSelfNodeUnexplored = mutableState.WasSelfNodeUnexplored;
            BgColor = mutableState.BgColor;
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

        private MutableTraversalState ToMutable() {
            return new MutableTraversalState {
                UnlockedGates = new HashSet<Gate>(UnlockedGates),
                VisitedNodesCurrentRun = new HashSet<INode>(VisitedNodesCurrentRun),
                VisitedNodesOverall = new HashSet<INode>(VisitedNodesOverall),
                CurrentNode = CurrentNode,
                NodeForTimeout = NodeForTimeout,
                ShowCountdown = ShowCountdown,
                CountdownValue = CountdownValue,
                WasSelfNodeUnexplored = WasSelfNodeUnexplored,
                BgColor = BgColor,
            };
        }

        public TraversalState Advance(INode next) {
            MutableTraversalState mutableState = ToMutable();

            mutableState.VisitedNodesCurrentRun.Add(next);
            mutableState.WasSelfNodeUnexplored = mutableState.VisitedNodesOverall.Add(next);

            if (mutableState is { ShowCountdown: true, CountdownValue: not null }) {
                mutableState.CountdownValue = Mathf.Max(0, mutableState.CountdownValue.Value - CurrentNode.Cost);
            }

            if (mutableState is { ShowCountdown: true, CountdownValue: 0, NodeForTimeout: not null }) {
                next = mutableState.NodeForTimeout;
                mutableState.NodeForTimeout = null;
            }
            
            next.ApplyToState(ref mutableState);
            TraversalState nextState = new(mutableState);
            return nextState;
        }
    }
}