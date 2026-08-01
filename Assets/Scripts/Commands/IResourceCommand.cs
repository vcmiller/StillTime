using System.Collections.Generic;
using StillTime.Nodes;

namespace StillTime.Commands {
    public interface IResourceCommand {
        public void CreateResources(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);
    }
}
