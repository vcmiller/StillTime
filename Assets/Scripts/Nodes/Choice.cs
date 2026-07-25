using System.Collections.Generic;

namespace Nodes {
    public class Choice {
        public string Text { get; }
        
        public INode Next { get; }
        
        public bool BypassVisitedCheck { get; }
    
        public List<ICondition> Gates { get; } = new();

        public Choice(string text, INode next, bool bypassVisitedCheck) {
            Text = text;
            Next = next;
            BypassVisitedCheck = bypassVisitedCheck;
        }
    }
}