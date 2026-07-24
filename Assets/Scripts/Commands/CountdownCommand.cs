namespace Commands {
    public class CountdownCommand : Command {
        public bool Show { get; }
        
        public int? Value { get; }

        public CountdownCommand(int lineNumber, string line, bool show, int? value) : base(lineNumber, line) {
            Show = show;
            Value = value;
        }
    }
}