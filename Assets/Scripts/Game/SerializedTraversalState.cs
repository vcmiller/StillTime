using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace StillTime.Game {
    public class SerializedTraversalState {
        public Dictionary<string, JValue> Variables { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, JValue> RunVariables { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Dictionary<string, JValue> GlobalVariables { get; set; }

        public List<string> VisitedNodesCurrentRun { get; set; }
        public List<string> VisitedNodesOverall { get; set; }
        public string CurrentNode { get; set; }
        public string NodeForTimeout { get; set; }
        public bool ShowCountdown { get; set; }
        public int? CountdownValue { get; set; }
        public string BgColor { get; set; }
        public bool WasCurrentStateUnexplored { get; set; }

        public SerializedTraversalState Clone() {
            return new SerializedTraversalState {
                Variables = new Dictionary<string, JValue>(Variables),
                RunVariables = RunVariables != null ? new Dictionary<string, JValue>(RunVariables) : null,
                GlobalVariables = GlobalVariables != null ? new Dictionary<string, JValue>(GlobalVariables) : null,
                VisitedNodesCurrentRun = new List<string>(VisitedNodesCurrentRun),
                VisitedNodesOverall = new List<string>(VisitedNodesOverall),
                CurrentNode = CurrentNode,
                NodeForTimeout = NodeForTimeout,
                ShowCountdown = ShowCountdown,
                CountdownValue = CountdownValue,
                BgColor = BgColor,
                WasCurrentStateUnexplored = WasCurrentStateUnexplored,
            };
        }
    }
}
