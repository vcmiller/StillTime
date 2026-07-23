namespace Commands {
    public class CostCommand : Command {
        public float Cost { get; }

        public CostCommand(int lineNumber, string line, float cost) : base(lineNumber, line) {
            Cost = cost;
        }
    }
}