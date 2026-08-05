
using StillTime.Sts.Nodes;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Resources {
    public class Speaker : Resource {
        public StsColor Color { get; }
        public string Text { get; }

        public Speaker(string identifier, StsColor color, string text) : base(identifier) {
            Color = color;
            Text = text;
        }
    }
}
