using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands {
    public class ElseCommand : Command {
        public List<ISequentialCommand> Commands { get; } = new();

        public ElseCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public override void GatherSubCommands(ref CommandGatheringState state) {
            CommandUtility.GatherSubCommands(this, ref state, Commands, true, true);
        }
    }
}
