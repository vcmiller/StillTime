namespace Commands {
    public class UnlockCommand : Command {
        public string GateName { get; }

        public UnlockCommand(int lineNumber, string line, string gateName) : base(lineNumber, line) {
            GateName = gateName;
        }
    }
}