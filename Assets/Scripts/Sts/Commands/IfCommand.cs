using System.Collections.Generic;
using System.Linq;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class IfCommand : Command, ISequentialCommand {
        public IReadOnlyList<string> Conditions { get; }

        public List<Command> Commands { get; } = new();

        public List<ElseIfCommand> ElseIfCommands { get; } = new();

        public ElseCommand ElseCommand { get; set; }

        public IfCommand(int lineNumber, string line, IReadOnlyList<string> conditions) : base(lineNumber, line) {
            Conditions = conditions;
        }

        public void ApplyToSequence(
            ref ISequentialNode nextNode,
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary,
            List<INode> createdNodes) {
            List<ICondition> conditions =
                Conditions.Select(str => CommandUtility.ProcessCondition(this, str, resourceDictionary))
                          .ToList();

            EmptyNode convergence = null;

            IfNode firstIfNode = new(conditions) {
                TrueBranch =
                    CreateBranch(Commands, ref convergence, createdNodes, nodeDictionary, resourceDictionary),
            };
            createdNodes.Add(firstIfNode);

            IfNode lastIfNode = firstIfNode;
            foreach (ElseIfCommand elseIf in ElseIfCommands) {
                List<ICondition> elseIfConditions =
                    elseIf.Conditions.Select(str => CommandUtility.ProcessCondition(this, str, resourceDictionary))
                          .ToList();

                EmptyNode branch =
                    CreateBranch(elseIf.Commands, ref convergence, createdNodes, nodeDictionary, resourceDictionary);

                IfNode nextIfNode = new(elseIfConditions) {
                    TrueBranch = branch,
                };
                createdNodes.Add(nextIfNode);

                lastIfNode.FalseBranch = nextIfNode;
                lastIfNode = nextIfNode;
            }

            if (ElseCommand != null) {
                EmptyNode falseBranch =
                    CreateBranch(ElseCommand.Commands, ref convergence, createdNodes, nodeDictionary,
                                 resourceDictionary);

                lastIfNode.FalseBranch = falseBranch;
                createdNodes.Add(falseBranch);
            }

            nextNode.Next = firstIfNode;
            nextNode = convergence;
        }

        private static EmptyNode CreateBranch(List<Command> branchCommands,
                                              ref EmptyNode convergenceNode,
                                              List<INode> createdNodes,
                                              Dictionary<string, INode> nodeDictionary,
                                              Dictionary<string, Resource> resourceDictionary) {
            if (branchCommands.Count == 0) {
                CreateConvergenceNode(ref convergenceNode, createdNodes);
                return convergenceNode;
            }

            EmptyNode branchNode = new();

            ISequentialNode lastBranchNode = branchNode;
            CommandUtility.ProcessLinearNodes(
                ref lastBranchNode,
                branchCommands,
                nodeDictionary,
                resourceDictionary,
                createdNodes);

            if (lastBranchNode != null) {
                CreateConvergenceNode(ref convergenceNode, createdNodes);
                lastBranchNode.Next = convergenceNode;
            }

            return branchNode;
        }

        private static void CreateConvergenceNode(ref EmptyNode convergenceNode, List<INode> createdNodes) {
            if (convergenceNode != null) return;
            convergenceNode = new EmptyNode();
            createdNodes.Add(convergenceNode);
        }
    }
}
