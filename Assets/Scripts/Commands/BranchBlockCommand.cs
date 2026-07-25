using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class BranchBlockCommand : TextCommand {
        public List<ChoiceCommand> Choices { get; } = new();

        public BranchBlockCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }

        public override void ApplyToSequence(
            ref ISingleNextNode nextNode,
            IReadOnlyDictionary<string, Resource> resources, 
            IReadOnlyDictionary<string, INode> nodeDictionary,
            List<INode> createdNodes) {

            Speaker speaker = CommandUtility.GetSpeaker(this, resources);
            BranchNode branchNode = new(Text, speaker);
            foreach (ChoiceCommand choiceCommand in Choices) {
                branchNode.Choices.Add(ProcessChoice(choiceCommand, nodeDictionary, resources));
            }

            createdNodes.Add(branchNode);
            nextNode.Next = branchNode;
            nextNode = null;
        }

        private static Choice ProcessChoice(
            ChoiceCommand command,
            IReadOnlyDictionary<string, INode> nodesByIdentifier,
            IReadOnlyDictionary<string, Resource> resources) {
            if (!nodesByIdentifier.TryGetValue(command.TargetLabel, out INode choiceTarget)) {
                throw new ParsingException(command.LineNumber, command.Line, "Invalid target label");
            }

            Choice choice = new(command.Text, choiceTarget, command.AlwaysAllow);
            foreach (string condStr in command.Conditions) {
                ICondition cond = CommandUtility.ProcessCondition(command, condStr, resources);
                choice.Conditions.Add(cond);
            }

            return choice;
        }
    }
}