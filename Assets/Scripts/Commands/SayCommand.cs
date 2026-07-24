namespace Commands {
    public class SayCommand : TextCommand {
        public SayCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) {
        }
    }
}