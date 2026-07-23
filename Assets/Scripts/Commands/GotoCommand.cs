namespace Commands {
    public class GotoCommand : Command {
        public string TargetLabel { get; set; }

        public GotoCommand(int lineNumber, string line, string targetLabel) : base(lineNumber, line) {
            TargetLabel = targetLabel;
        }
    }
}