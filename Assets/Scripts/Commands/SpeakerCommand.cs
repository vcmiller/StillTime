using System.Collections.Generic;
using Nodes;
using UnityEngine;

namespace Commands {
    public class SpeakerCommand : Command, IResourceCommand {
        public string Name { get; }
        public Color Color { get; }
        public string Text { get; }

        public SpeakerCommand(int lineNumber, string line, string name, Color color, string text) :
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
