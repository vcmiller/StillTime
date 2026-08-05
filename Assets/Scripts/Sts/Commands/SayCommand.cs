using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class SayCommand : TextCommand, ISequentialCommand {
        public SayCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            Speaker speaker = CommandUtility.GetSpeaker(this, resourceDictionary);
            SayNode node = new(Text, speaker);
            createdNodes.Add(node);
            nextNode.Next = node;
            nextNode = node;
        }
    }
}
