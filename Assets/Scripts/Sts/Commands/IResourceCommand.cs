using System.Collections.Generic;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public interface IResourceCommand {
        public void CreateResources(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);
    }
}
