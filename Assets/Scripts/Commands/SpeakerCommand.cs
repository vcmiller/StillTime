using UnityEngine;

namespace Commands {
    public class SpeakerCommand : Command {
        public string Name { get; }
        public Color Color { get; }
        public string Text { get; }

        public SpeakerCommand(int lineNumber, string line, string name, Color color, string text) :
            base(lineNumber, line) {

            Name = name;
            Color = color;
            Text = text;
        }
    }
}