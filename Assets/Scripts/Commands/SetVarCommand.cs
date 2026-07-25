namespace Commands {
    public class SetVarCommand : Command {
        public string VarName { get; }
        public string Value { get; }

        public SetVarCommand(int lineNumber, string line, string varName, string value) : base(lineNumber, line) {
            VarName = varName;
            Value = value;
        }
    }
}