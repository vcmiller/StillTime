using System.Collections.Generic;
using Nodes;

namespace Commands {
    public interface ISequentialCommand {
        public void ApplyToSequence(
            ref ISequentialNode nextNode,
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary,
            List<INode> createdNodes);
    }
}
