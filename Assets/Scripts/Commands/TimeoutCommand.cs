using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class TimeoutCommand : Command {
        public string TargetLabel { get; }

        public TimeoutCommand(int lineNumber, string line, string targetLabel) : base(lineNumber, line) {
            TargetLabel = targetLabel;
        }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
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