using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class Command {
        public int LineNumber { get; }
        public string Line { get; }

        public Command(int lineNumber, string line) {
            LineNumber = lineNumber;
            Line = line;
        }

        public virtual void CreateResources(
            Dictionary<string, Resource> resources,
            Dictionary<string, INode> nodeDictionary) {
            
        }

        public virtual void ApplyToSequence(
            ref ISingleNextNode nextNode,
            IReadOnlyDictionary<string, Resource> resources,
            IReadOnlyDictionary<string, INode> nodeDictionary,
            List<INode> createdNodes) {
        }
    }
}