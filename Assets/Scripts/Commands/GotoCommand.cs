using System.Collections.Generic;
using System.Linq;
using Nodes;

namespace Commands {
    public class GotoCommand : Command {
        public string TargetLabel { get; }

        public bool ResetRunState { get; }

        public IReadOnlyList<string> Conditions { get; }

        public GotoCommand(int lineNumber, string line, string targetLabel, bool resetRunState,
                           IReadOnlyList<string> conditions) : base(lineNumber, line) {
            TargetLabel = targetLabel;
            ResetRunState = resetRunState;
            Conditions = conditions;
        }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            INode gotoTarget = CommandUtility.GetNode(this, TargetLabel, nodeDictionary);

            if (ResetRunState) {
                ResetRunNode resetNode = new() { Next = gotoTarget };
                gotoTarget = resetNode;
                createdNodes.Add(resetNode);
            }

            if (Conditions.Count > 0) {
                List<ICondition> conditions =
                    Conditions.Select(str => CommandUtility.ProcessCondition(this, str, resources))
                              .ToList();


                IfNode ifNode = new(conditions, gotoTarget);
                createdNodes.Add(ifNode);
                nextNode.Next = ifNode;
                nextNode = ifNode;
            } else {
                nextNode.Next = gotoTarget;
                nextNode = null;
            }
        }
    }
}