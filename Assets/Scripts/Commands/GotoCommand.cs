namespace Commands {
    public class GotoCommand : Command {
        public string TargetLabel { get; set; }
        
        public bool ResetRunState { get; set; }

        public GotoCommand(int lineNumber, string line, string targetLabel, bool resetRunState) : base(lineNumber, line) {
            TargetLabel = targetLabel;
            ResetRunState = resetRunState;
        }
    }
}