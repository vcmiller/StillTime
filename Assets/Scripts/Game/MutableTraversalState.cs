using System;
using System.Collections.Generic;
using Commands;
using Nodes;
using UnityEngine;

namespace Game {
    public struct MutableTraversalState {
        public Dictionary<Variable, object> RunVariables { get; set; }
        public Dictionary<Variable, object> GlobalVariables { get; set; }
        public HashSet<INode> VisitedNodesCurrentRun { get; set; }
        public HashSet<INode> VisitedNodesOverall { get; set; }
        public INode CurrentNode { get; set; }
        public INode NodeForTimeout { get; set; }
        public bool ShowCountdown { get; set; }
        public int? CountdownValue { get; set; }
        public bool WasSelfNodeUnexplored { get; set; }
        public Color BgColor { get; set; }

        public void SetVariableValue(Variable variable, object value) {
            if (variable.DefaultValue.GetType() != value.GetType()) {
                throw new ArgumentException($"Invalid value {value} specified for variable {variable}.");
            }

            Dictionary<Variable, object> dict = variable.Scope == VarScope.Global ? GlobalVariables : RunVariables;
            dict[variable] = value;
        }
    }
}