using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public interface IResourceCommand {
        public void CreateResources(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);

        public void ValidateResources(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary) { }
    }
}
