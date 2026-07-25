using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class SayCommand : TextCommand {
        public SayCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            Speaker speaker = CommandUtility.GetSpeaker(this, resources);
            SingleTextNode node = new(Text, speaker);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}