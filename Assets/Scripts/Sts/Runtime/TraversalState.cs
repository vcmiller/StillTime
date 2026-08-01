using System;
using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Runtime {
    public class TraversalState {
        public IReadOnlyDictionary<Variable, object> RunVariables { get; }

        public IReadOnlyDictionary<Variable, object> GlobalVariables { get; }

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
            IReadOnlyDictionary<Variable, object> runVariables,
            IReadOnlyDictionary<Variable, object> globalVariables,
            IEnumerable<INode> visitedNodesCurrentRun,
            IEnumerable<INode> visitedNodesOverall,
            StsColor bgColor,
            bool wasCurrentNodeUnexplored) {
            CurrentNode = node;
            NodeForTimeout = nodeForTimeout;
            ShowCountdown = showCountdown;
            CountdownValue = countdownValue;
            RunVariables = new Dictionary<Variable, object>(runVariables);
            GlobalVariables = new Dictionary<Variable, object>(globalVariables);
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
            RunVariables = mutableState.RunVariables;
            GlobalVariables = mutableState.GlobalVariables;
            VisitedNodesCurrentRun = new ReadOnlySet<INode>(mutableState.VisitedNodesCurrentRun);
            VisitedNodesOverall = new ReadOnlySet<INode>(mutableState.VisitedNodesOverall);
            BgColor = mutableState.BgColor;
            WasCurrentNodeUnexplored = wasCurrentNodeUnexplored;
        }

        public T GetVariableValue<T>(Variable variable) {
            if (typeof(T) != variable.DefaultValue.GetType()) {
                throw new InvalidOperationException($"Trying to get variable {variable.Identifier} value with invalid type {typeof(T)}");
            }
            return GetVariableValue(variable) is T t ? t : default;
        }

        public object GetVariableValue(Variable variable) {
            return RunVariables.GetValueOrDefault(variable) ??
                   GlobalVariables.GetValueOrDefault(variable) ??
                   variable.DefaultValue;
        }

        private MutableTraversalState ToMutable() {
            return new MutableTraversalState {
                RunVariables = new Dictionary<Variable, object>(RunVariables),
                GlobalVariables = new Dictionary<Variable, object>(GlobalVariables),
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
