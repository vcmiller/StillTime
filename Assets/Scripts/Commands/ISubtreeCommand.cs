using System.Collections.Generic;
using StillTime.Nodes;

namespace StillTime.Commands {
    public interface ISubtreeCommand {
        public void BuildNodeTree(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary);
    }
}
