using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public abstract class TextCommand : Command {
        public string Speaker { get; }
        public string Text { get; }

        public TextCommand(int lineNumber, string line, string speaker, string text) : base(lineNumber, line) {
            Speaker = speaker;
            Text = text;
        }

        public Speaker GetSpeaker(GraphData graphData) {
            if (string.IsNullOrWhiteSpace(Speaker)) {
                return null;
            }

            return graphData.GetResource<Speaker>(this, Speaker);
        }
    }
}
