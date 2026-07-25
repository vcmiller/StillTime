using System.Collections.Generic;
using Nodes;
using UnityEngine;

namespace Game {
    public struct MutableTraversalState {
        public HashSet<Gate> UnlockedGates { get; set; }
        public HashSet<INode> VisitedNodesCurrentRun { get; set; }
        public HashSet<INode> VisitedNodesOverall { get; set; }
        public INode CurrentNode { get; set; }
        public INode NodeForTimeout { get; set; }
        public bool ShowCountdown { get; set; }
        public int? CountdownValue { get; set; }
        public bool WasSelfNodeUnexplored { get; set; }
        public Color BgColor { get; set; }
    }
}