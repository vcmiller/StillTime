using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Commands {
    public class SpeakerCommand : Command, IResourceCommand {
        public string Name { get; }
        public StsColor Color { get; }
        public string Text { get; }

        public SpeakerCommand(int lineNumber, string line, string name, StsColor color, string text) :
            base(lineNumber, line) {
            Name = name;
            Color = color;
            Text = text;
        }

        public void CreateResources(Dictionary<string, Resource> resources,
                                    Dictionary<string, INode> nodeDictionary) {
            Speaker speaker = new(Name, Color, Text);
            resources.Add(Name, speaker);
        }
    }
}
