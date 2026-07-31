using System.Collections.Generic;
using Nodes;

namespace Commands {
    public interface IBranchSubCommand {
        public IBranchOption CreateBranchOption(
            Dictionary<string, Resource> resourceDictionary,
            Dictionary<string, INode> nodeDictionary);
    }
}
