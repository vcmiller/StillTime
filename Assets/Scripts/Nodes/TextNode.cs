namespace Nodes {
    public abstract class TextNode : Node {
        public string Text { get; set; }
        public Speaker Speaker { get; set; }

        public TextNode(string text, Speaker speaker) {
            Text = text;
            Speaker = speaker;
        }
    }
}