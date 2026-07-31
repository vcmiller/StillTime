using System.Collections.Generic;
using Game;

namespace Nodes {
    public class Choice : IBranchOption {
        public string Text { get; }

        public INode Next { get; }

        public bool BypassVisitedCheck { get; }

        public List<ICondition> Conditions { get; } = new();

        public Choice(string text, INode next, bool bypassVisitedCheck) {
            Text = text;
            Next = next;
            BypassVisitedCheck = bypassVisitedCheck;
        }

        public string GetText(TraversalState state) {
            return Text;
        }

        public bool IsAvailable(TraversalState state) {
            if (!BypassVisitedCheck && state.VisitedNodesCurrentRun.Contains(Next)) return false;
            if (!Conditions.TrueForAll(g => g.CheckCondition(state))) return false;
            return true;
        }

        public INode GetNextNode(TraversalState state) {
            return Next;
        }
    }
}
