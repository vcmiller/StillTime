using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class EndCommand : Command, ISequentialCommand {
        public EndCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            nextNode = null;
        }
    }
}
