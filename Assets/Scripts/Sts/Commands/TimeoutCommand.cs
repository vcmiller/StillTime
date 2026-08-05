using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class TimeoutCommand : Command, ISequentialCommand {
        public string TargetLabel { get; }

        public TimeoutCommand(int lineNumber, string line, string targetLabel) : base(lineNumber, line) {
            TargetLabel = targetLabel;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            INode timeoutTarget = TargetLabel != null
                ? CommandUtility.GetNode(this, TargetLabel, nodeDictionary)
                : null;
            TimeoutNode timeoutNode = new(timeoutTarget);
            createdNodes.Add(timeoutNode);
            nextNode.Next = timeoutNode;
            nextNode = timeoutNode;
        }
    }
}
