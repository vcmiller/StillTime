using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class EndCommand : Command {
        public EndCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                              IReadOnlyDictionary<string, Resource> resources,
                                              IReadOnlyDictionary<string, INode> nodeDictionary,
                                              List<INode> createdNodes) {
            nextNode = null;
        }
    }
}