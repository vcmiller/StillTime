using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands.Interfaces {
    public interface ICommand {
        public int LineNumber { get; }

        public string Line { get; }

        public void GatherSubCommands(ref CommandGatheringState state);
    }
}
