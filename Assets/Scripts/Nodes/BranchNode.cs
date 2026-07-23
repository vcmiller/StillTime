using System.Collections.Generic;

namespace Nodes {
    public class BranchNode : TextNode {
        public List<Choice> Choices { get; } = new();
    }
}