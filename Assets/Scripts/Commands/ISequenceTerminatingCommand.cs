namespace StillTime.Commands {
    public interface ISequenceTerminatingCommand {
        public bool IsTerminating => true;
    }
}
