namespace Commands {
    public class IncrVarCommand : Command {
        public string VarName { get; }
        
        public int Value { get; }
        
        public IncrVarCommand(int lineNumber, string line, string varName, int value) : base(lineNumber, line) {
            VarName = varName;
            Value = value;
        }
    }
}