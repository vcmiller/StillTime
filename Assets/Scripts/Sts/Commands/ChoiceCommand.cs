using System.Collections.Generic;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Commands {
    public class ChoiceCommand : TextCommand, IBranchSubCommand {
        public string TargetLabel { get; }
        public IReadOnlyList<string> Conditions { get; }
        public bool AlwaysAllow { get; }

        public ChoiceCommand(
            int lineNumber,
            string line,
            string text,
            string targetLabel,
            bool alwaysAllow,
            IReadOnlyList<string> conditions) :
            base(lineNumber, line, null, text) {
            TargetLabel = targetLabel;
            Conditions = conditions;
            AlwaysAllow = alwaysAllow;
        }

        public IBranchOption CreateBranchOption(Dictionary<string, Resource> resourceDictionary,
                                                Dictionary<string, INode> nodeDictionary) {

            if (!nodeDictionary.TryGetValue(TargetLabel, out INode targetNode)) {
                throw new ParsingException(LineNumber, Line, $"Choice command has invalid target label '{TargetLabel}'");
            }

            Choice choice = new(Text, targetNode, AlwaysAllow);

            foreach (string condStr in Conditions) {
                ICondition cond = CommandUtility.ProcessCondition(this, condStr, resourceDictionary);
                choice.Conditions.Add(cond);
            }

            return choice;
        }
    }
}
