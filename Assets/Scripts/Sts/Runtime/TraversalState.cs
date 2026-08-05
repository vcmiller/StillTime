using System;
using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Runtime {
    public class TraversalState {
        public IReadOnlyDictionary<Variable, StsValue> Variables { get; }

        public ReadOnlySet<INode> VisitedNodesCurrentRun { get; }

        public ReadOnlySet<INode> VisitedNodesOverall { get; }

        public INode CurrentNode { get; }

        public INode NodeForTimeout { get; }

        public bool ShowCountdown { get; }

        public int? CountdownValue { get; }

        public StsColor BgColor { get; }

        public bool WasCurrentNodeUnexplored { get; }

        public TraversalState(
            INode node,
            INode nodeForTimeout,
            bool showCountdown,
            int? countdownValue,
            IReadOnlyDictionary<Variable, StsValue> variables,
            IEnumerable<INode> visitedNodesCurrentRun,
            IEnumerable<INode> visitedNodesOverall,
            StsColor bgColor,
            bool wasCurrentNodeUnexplored) {
            CurrentNode = node;
            NodeForTimeout = nodeForTimeout;
            ShowCountdown = showCountdown;
            CountdownValue = countdownValue;
            Variables = new Dictionary<Variable, StsValue>(variables);
            VisitedNodesCurrentRun = new ReadOnlySet<INode>(visitedNodesCurrentRun);
            VisitedNodesOverall = new ReadOnlySet<INode>(visitedNodesOverall);
            BgColor = bgColor;
            WasCurrentNodeUnexplored = wasCurrentNodeUnexplored;
        }

        public TraversalState(MutableTraversalState mutableState, bool wasCurrentNodeUnexplored) {
            CurrentNode = mutableState.CurrentNode;
            NodeForTimeout = mutableState.NodeForTimeout;
            ShowCountdown = mutableState.ShowCountdown;
            CountdownValue = mutableState.CountdownValue;
            Variables = mutableState.Variables;
            VisitedNodesCurrentRun = new ReadOnlySet<INode>(mutableState.VisitedNodesCurrentRun);
            VisitedNodesOverall = new ReadOnlySet<INode>(mutableState.VisitedNodesOverall);
            BgColor = mutableState.BgColor;
            WasCurrentNodeUnexplored = wasCurrentNodeUnexplored;
        }

        public StsValue GetVariableValue(Variable variable) {
            return Variables.TryGetValue(variable, out StsValue value) ? value : variable.DefaultValue;
        }

        private MutableTraversalState ToMutable() {
            return new MutableTraversalState {
                Variables = new Dictionary<Variable, StsValue>(Variables),
                VisitedNodesCurrentRun = new HashSet<INode>(VisitedNodesCurrentRun),
                VisitedNodesOverall = new HashSet<INode>(VisitedNodesOverall),
                CurrentNode = CurrentNode,
                NodeForTimeout = NodeForTimeout,
                ShowCountdown = ShowCountdown,
                CountdownValue = CountdownValue,
                BgColor = BgColor,
            };
        }

        public TraversalState Advance(INode next) {
            MutableTraversalState mutableState = ToMutable();

            if (mutableState is { ShowCountdown: true, CountdownValue: not null }) {
                mutableState.CountdownValue = Math.Max(0, mutableState.CountdownValue.Value - CurrentNode.Cost);
            }

            if (mutableState is { ShowCountdown: true, CountdownValue: 0, NodeForTimeout: not null }) {
                next = mutableState.NodeForTimeout;
                mutableState.NodeForTimeout = null;
            }

            mutableState.CurrentNode = next;

            bool unexplored = false;
            INode prevNode;
            do {
                prevNode = mutableState.CurrentNode;
                mutableState.VisitedNodesCurrentRun.Add(mutableState.CurrentNode);
                unexplored |= mutableState.VisitedNodesOverall.Add(mutableState.CurrentNode);
                mutableState.CurrentNode.ApplyToState(ref mutableState);
            } while (mutableState.CurrentNode != prevNode);

            TraversalState nextState = new(mutableState, unexplored);
            return nextState;
        }
    }
}
