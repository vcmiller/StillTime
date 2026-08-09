using System.Collections.Generic;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;

namespace StillTime.Sts.Commands.Interfaces {
    public interface IBranchSubCommand : ICommand {
        public void CreateBranchOptions(GraphData graphData, List<IBranchOption> options);
    }
}
