namespace Commands {
    public class TimeoutCommand : Command {
        public string TargetLabel { get; }
        
        public TimeoutCommand(int lineNumber, string line, string targetLabel) : base(lineNumber, line) {
            TargetLabel = targetLabel;
        }
    }
}