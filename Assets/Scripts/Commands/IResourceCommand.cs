using System.Collections.Generic;
using Nodes;

namespace Commands {
    public interface IResourceCommand {
        public void CreateResources(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);
    }
}
