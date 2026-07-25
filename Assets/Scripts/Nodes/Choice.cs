using System.Collections.Generic;

namespace Nodes {
    public class Choice {
        public string Text { get; }
        
        public INode Next { get; }
        
        public bool AlwaysAllow { get; }
    
        public List<Variable> Gates { get; } = new();

        public Choice(string text, INode next, bool alwaysAllow) {
            Text = text;
            Next = next;
            AlwaysAllow = alwaysAllow;
        }
    }
}