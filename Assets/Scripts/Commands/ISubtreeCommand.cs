using System.Collections.Generic;
using Nodes;

namespace Commands {
    public interface ISubtreeCommand {
        public void BuildNodeTree(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary);
    }
}
