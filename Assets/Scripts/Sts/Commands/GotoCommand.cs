using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class GotoCommand : Command, ISequentialCommand {
        public string TargetLabel { get; }

        public bool ResetRunState { get; }

        public GotoCommand(int lineNumber, string line, string targetLabel, bool resetRunState) :
            base(lineNumber, line) {
            TargetLabel = targetLabel;
            ResetRunState = resetRunState;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {

            INode gotoTarget = CommandUtility.GetNode(this, TargetLabel, nodeDictionary);

            if (ResetRunState) {
                ResetRunNode resetNode = new() { Next = gotoTarget };
                gotoTarget = resetNode;
                createdNodes.Add(resetNode);
            }

            nextNode.Next = gotoTarget;
            nextNode = null;
        }
    }
}
