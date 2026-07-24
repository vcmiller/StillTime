using UnityEngine;

namespace Nodes {
    public class Speaker {
        public string Name { get; }
        public Color Color { get; }
        public string Text { get; }

        public Speaker(string name, Color color, string text) {
            Name = name;
            Color = color;
            Text = text;
        }
    }
}