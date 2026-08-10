using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands {
    public class ElseIfCommand : Command {
        public string Condition { get; }

        public List<ISequentialCommand> Commands { get; } = new();

        public ElseIfCommand(int lineNumber, string line, string condition) : base(lineNumber, line) {
            Condition = condition;
        }

        public override void GatherSubCommands(ref CommandGatheringState state) {
            CommandUtility.GatherSubCommands(this, ref state, Commands, false, true);
        }
    }
}
