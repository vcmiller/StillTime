using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands.Interfaces {
    public interface IResourceCommand : ICommand {
        public void CreateResources(GraphData graphData);

        public void ValidateResources(GraphData graphData) { }
    }
}
