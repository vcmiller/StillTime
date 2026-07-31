using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class EndCommand : Command, ISequentialCommand, ISequenceTerminatingCommand {
        public EndCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            nextNode = null;
        }
    }
}
