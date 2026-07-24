using System.Collections.Generic;

namespace Game {
    public class SerializedTraversalState {
        public List<string> UnlockedGates { get; set; }
        public List<string> VisitedNodesCurrentRun { get; set; }
        public List<string> VisitedNodesOverall { get; set; }
        public string CurrentNode { get; set; }
        public string NodeForTimeout { get; set; }
        public bool ShowCountdown { get; set; }
        public int? CountdownValue { get; set; }
        public bool WasSelfNodeUnexplored { get; set; }
        public string BgColor { get; set; }
    }
}