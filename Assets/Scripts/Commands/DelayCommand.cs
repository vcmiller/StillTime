namespace Commands {
    public class DelayCommand : Command {
        public float Time { get; }

        public DelayCommand(int lineNumber, string line, float time) : base(lineNumber, line) {
            Time = time;
        }
    }
}