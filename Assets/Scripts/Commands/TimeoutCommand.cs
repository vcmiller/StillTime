namespace Commands {
    public class TimeoutCommand : Command {
        public string Target { get; }
        
        public TimeoutCommand(int lineNumber, string line, string target) : base(lineNumber, line) {
            Target = target;
        }
    }
}