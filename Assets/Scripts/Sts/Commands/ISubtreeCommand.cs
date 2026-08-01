using System.Collections.Generic;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public interface ISubtreeCommand {
        public void BuildNodeTree(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary);
    }
}
