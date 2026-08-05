using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public interface ISubtreeCommand {
        public void BuildNodeTree(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary);
    }
}
