using System.Collections.Generic;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands {
    public class BranchBlockCommand : TextCommand, ISequentialCommand {
        public List<IBranchSubCommand> SubCommands { get; } = new();

        public BranchBlockCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }

        public void ApplyToSequence(
            ref ISequentialNode nextNode,
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary,
            List<INode> createdNodes) {

            Speaker speaker = CommandUtility.GetSpeaker(this, resourceDictionary);
            BranchNode branchNode = new(Text, speaker);
            foreach (IBranchSubCommand subCommand in SubCommands) {
                IBranchOption option = subCommand.CreateBranchOption(resourceDictionary, nodeDictionary);
                if (option == null) continue;
                branchNode.Options.Add(option);
            }

            createdNodes.Add(branchNode);
            nextNode.Next = branchNode;
            nextNode = null;
        }
    }
}
