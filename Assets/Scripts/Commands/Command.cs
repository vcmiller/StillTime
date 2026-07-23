namespace Commands {
    public class Command {
        public int LineNumber { get; }
        public string Line { get; }

        public Command(int lineNumber, string line) {
            LineNumber = lineNumber;
            Line = line;
        }
    }
}