namespace Commands {
    public class GateCommand : Command {
        public string GateName { get; }

        public GateCommand(int lineNumber, string line, string gateName) : base(lineNumber, line) {
            GateName = gateName;
        }
    }
}