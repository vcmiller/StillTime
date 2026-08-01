using UnityEngine;

namespace StillTime.Nodes {
    public class Speaker : Resource {
        public Color Color { get; }
        public string Text { get; }

        public Speaker(string identifier, Color color, string text) : base(identifier) {
            Color = color;
            Text = text;
        }
    }
}