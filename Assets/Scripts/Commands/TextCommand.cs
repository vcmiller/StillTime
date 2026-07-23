namespace Commands {
    public class TextCommand : Command {
        public string Text { get; }

        public TextCommand(int lineNumber, string line, string text) : base(lineNumber, line) {
            Text = text;
        }
    }
}