using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Collections.Generic;
using Nodes;
using UnityEngine;
using Utility;

namespace Game {
    public class TraversalState {
        public IReadOnlyDictionary<Variable, object> RunVariables { get; }
        
        public IReadOnlyDictionary<Variable, object> GlobalVariables { get; }
        
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
            IReadOnlyDictionary<Variable, object> runVariables,
            IReadOnlyDictionary<Variable, object> globalVariables,
            IEnumerable<INode> visitedNodesCurrentRun,
            IEnumerable<INode> visitedNodesOverall,
            bool wasSelfNodeUnexplored,
            Color bgColor) {
            CurrentNode = node;
            NodeForTimeout = nodeForTimeout;
            ShowCountdown = showCountdown;
            CountdownValue = countdownValue;
            RunVariables = new Dictionary<Variable, object>(runVariables);
            GlobalVariables = new Dictionary<Variable, object>(globalVariables);
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
            RunVariables = mutableState.RunVariables;
            GlobalVariables = mutableState.GlobalVariables;
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
            if (!choice.Gates.TrueForAll(GetVariableValue<bool>)) return false;
            return true;
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
                WasSelfNodeUnexplored = WasSelfNodeUnexplored,
                BgColor = BgColor,
            };
        }

        public TraversalState Advance(INode next) {
            MutableTraversalState mutableState = ToMutable();

            if (mutableState is { ShowCountdown: true, CountdownValue: not null }) {
                mutableState.CountdownValue = Mathf.Max(0, mutableState.CountdownValue.Value - CurrentNode.Cost);
            }

            if (mutableState is { ShowCountdown: true, CountdownValue: 0, NodeForTimeout: not null }) {
                next = mutableState.NodeForTimeout;
                mutableState.NodeForTimeout = null;
            }

            mutableState.CurrentNode = next;
            mutableState.VisitedNodesCurrentRun.Add(next);
            mutableState.WasSelfNodeUnexplored = mutableState.VisitedNodesOverall.Add(next);
            
            mutableState.CurrentNode.ApplyToState(ref mutableState);
            TraversalState nextState = new(mutableState);
            return nextState;
        }
    }
}