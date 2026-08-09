using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class BranchCommand : TextCommand, ISequentialCommand {
        public List<IBranchSubCommand> SubCommands { get; } = new();

        public BranchCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }

        public override void GatherSubCommands(ref CommandGatheringState state) {
            CommandUtility.GatherSubCommands(this, ref state, SubCommands);
        }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            Speaker speaker = GetSpeaker(graphData);
            BranchNode branchNode = new(Text, speaker);

            foreach (IBranchSubCommand subCommand in SubCommands) {
                subCommand.CreateBranchOptions(graphData, branchNode.Options);
            }

            builder.Append(branchNode);
        }
    }
}
