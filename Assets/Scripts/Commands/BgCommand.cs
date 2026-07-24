using UnityEngine;

namespace Commands {
    public class BgCommand : Command {
        public Color Color { get; }
        public float Time { get; }

        public BgCommand(int lineNumber, string line, Color color, float time) : base(lineNumber, line) {
            Color = color;
            Time = time;
        }
    }
}