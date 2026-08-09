using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class SayCommand : TextCommand, ISequentialCommand {
        public SayCommand(int lineNumber, string line, string speaker, string text) :
            base(lineNumber, line, speaker, text) { }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            Speaker speaker = GetSpeaker(graphData);
            SayNode node = new(Text, speaker);
            builder.Append(node);
        }
    }
}
