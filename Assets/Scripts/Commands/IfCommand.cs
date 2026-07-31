using System.Collections.Generic;
using System.Linq;
using Nodes;

namespace Commands {
    public class IfCommand : Command, ISequentialCommand, ISequenceTerminatingCommand {
        public IReadOnlyList<string> Conditions { get; }

        public List<Command> Commands { get; } = new();

        public List<ElseIfCommand> ElseIfCommands { get; } = new();

        public ElseCommand ElseCommand { get; set; }

        public bool IsTerminating =>
            IsSequenceTerminating(Commands) &&
            IsSequenceTerminating(ElseCommand?.Commands) &&
            ElseIfCommands.All(e => IsSequenceTerminating(e.Commands));

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

                lastIfNode.FalseBranch = nextIfNode;
                lastIfNode = nextIfNode;
            }

            if (ElseCommand != null) {
                EmptyNode falseBranch =
                    CreateBranch(ElseCommand.Commands, ref convergence, createdNodes, nodeDictionary, resourceDictionary);

                lastIfNode.FalseBranch = falseBranch;
            }

            nextNode.Next = firstIfNode;
            nextNode = convergence;
        }

        private EmptyNode CreateBranch(List<Command> branchCommands,
                                       ref EmptyNode convergenceNode,
                                       List<INode> createdNodes,
                                       Dictionary<string, INode> nodeDictionary,
                                       Dictionary<string, Resource> resourceDictionary) {
            if (branchCommands.Count == 0) {
                convergenceNode ??= new EmptyNode();
                createdNodes.Add(convergenceNode);
                return convergenceNode;
            }

            EmptyNode branchNode = new();

            CommandUtility.ProcessLinearNodes(
                branchNode,
                Commands,
                nodeDictionary,
                resourceDictionary,
                createdNodes,
                out ISequentialNode lastBranchNode);

            if (lastBranchNode != null) {
                convergenceNode ??= new EmptyNode();
                createdNodes.Add(convergenceNode);
                lastBranchNode.Next = convergenceNode;
            }

            return branchNode;
        }

        private bool IsSequenceTerminating(List<Command> sequence) {
            return sequence is { Count: > 0 } &&
                   sequence.Any(c => c is ISequenceTerminatingCommand { IsTerminating: true });
        }
    }
}
