using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
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

        public void CreateResources(GraphData graphData) {
            Speaker speaker = new(Name, Color, Text);
            graphData.Resources.Add(Name, speaker);
        }
    }
}
