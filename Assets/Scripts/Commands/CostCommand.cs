namespace Commands {
    public class CostCommand : Command {
        public int Cost { get; }

        public CostCommand(int lineNumber, string line, int cost) : base(lineNumber, line) {
            Cost = cost;
        }
    }
}