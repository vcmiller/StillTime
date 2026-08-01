
using StillTime.Sts.Utility;

namespace StillTime.Sts.Nodes {
    public class Speaker : Resource {
        public StsColor Color { get; }
        public string Text { get; }

        public Speaker(string identifier, StsColor color, string text) : base(identifier) {
            Color = color;
            Text = text;
        }
    }
}
