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

        public SerializedTraversalState Clone() {
            return new SerializedTraversalState {
                UnlockedGates = new List<string>(UnlockedGates),
                VisitedNodesCurrentRun = new List<string>(VisitedNodesCurrentRun),
                VisitedNodesOverall = new List<string>(VisitedNodesOverall),
                CurrentNode = CurrentNode,
                NodeForTimeout = NodeForTimeout,
                ShowCountdown = ShowCountdown,
                CountdownValue = CountdownValue,
                WasSelfNodeUnexplored = WasSelfNodeUnexplored,
                BgColor = BgColor,
            };
        }
    }
}