using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Game {
    public class SerializedTraversalState {
        public Dictionary<string, JValue> RunVariables { get; set; }
        public Dictionary<string, JValue> GlobalVariables { get; set; }
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
                RunVariables = new Dictionary<string, JValue>(RunVariables),
                GlobalVariables = new Dictionary<string, JValue>(GlobalVariables),
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