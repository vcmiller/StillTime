using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands.Interfaces {
    public interface ISequentialCommand : ICommand {
        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData);
    }
}
