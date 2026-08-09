using System;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;

namespace StillTime.Sts.Commands {
    public class EndCommand : Command, ISequentialCommand {
        public EndCommand(int lineNumber, string line) : base(lineNumber, line) { }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            throw new InvalidOperationException("End command not expected to be applied.");
        }
    }
}
