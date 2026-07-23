using System.Collections.Generic;

namespace Nodes {
    public class Choice {
        public string Text { get; set; }
        
        public INode Next { get; set; }
    
        public List<Gate> Gates { get; } = new();
    }
}