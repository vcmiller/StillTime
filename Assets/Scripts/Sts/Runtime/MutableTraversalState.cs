using System;
using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Runtime {
    public struct MutableTraversalState {
        public Dictionary<Variable, StsValue> Variables { get; set; }
        public HashSet<INode> VisitedNodesCurrentRun { get; set; }
        public HashSet<INode> VisitedNodesOverall { get; set; }
        public INode CurrentNode { get; set; }
        public INode NodeForTimeout { get; set; }
        public bool ShowCountdown { get; set; }
        public int? CountdownValue { get; set; }
        public StsColor BgColor { get; set; }

        public void SetVariableValue(Variable variable, StsValue value) {
            if (variable.DefaultValue.GetType() != value.GetType()) {
                throw new ArgumentException($"Invalid value {value} specified for variable {variable}.");
            }

            Variables[variable] = value;
        }

        public StsValue GetVariableValue(Variable variable) {
            return Variables.TryGetValue(variable, out StsValue value) ? value : variable.DefaultValue;
        }
    }
}
