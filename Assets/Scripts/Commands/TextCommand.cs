namespace StillTime.Commands {
    public abstract class TextCommand : Command {
        public string Speaker { get; }
        public string Text { get; }

        public TextCommand(int lineNumber, string line, string speaker, string text) : base(lineNumber, line) {
            Speaker = speaker;
            Text = text;
        }
    }
}