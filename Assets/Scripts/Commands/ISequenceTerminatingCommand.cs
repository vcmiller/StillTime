namespace Commands {
    public interface ISequenceTerminatingCommand {
        public bool IsTerminating => true;
    }
}
