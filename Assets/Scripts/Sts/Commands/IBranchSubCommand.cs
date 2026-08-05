using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public interface IBranchSubCommand {
        public IBranchOption CreateBranchOption(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);
    }
}
