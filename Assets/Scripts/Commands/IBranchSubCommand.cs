using System.Collections.Generic;
using StillTime.Nodes;

namespace StillTime.Commands {
    public interface IBranchSubCommand {
        public IBranchOption CreateBranchOption(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);
    }
}
