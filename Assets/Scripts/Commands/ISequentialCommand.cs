using System.Collections.Generic;
using Nodes;

namespace Commands {
    public interface ISequentialCommand {
        public void ApplyToSequence(
            ref ISingleNextNode nextNode,
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary,
            List<INode> createdNodes);
    }
}
