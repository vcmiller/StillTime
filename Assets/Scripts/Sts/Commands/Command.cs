using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands {
    public class Command : ICommand {
        public int LineNumber { get; }
        public string Line { get; }

        public Command(int lineNumber, string line) {
            LineNumber = lineNumber;
            Line = line;
        }

        public virtual void GatherSubCommands(ref CommandGatheringState state) { }
    }
}
