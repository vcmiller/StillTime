using System.Collections.Generic;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public interface ISequentialCommand {
        public void ApplyToSequence(
            ref ISequentialNode nextNode,
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary,
            List<INode> createdNodes);
    }
}
