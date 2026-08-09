using System.Collections.Generic;
using StillTime.Sts.Runtime;

namespace StillTime.Sts.Nodes {
    public class Choice : IBranchOption {
        public string Text { get; }

        public INode Next { get; }

        public List<ICondition> Conditions { get; } = new();

        public Choice(string text, INode next) {
            Text = text;
            Next = next;
        }

        public virtual string GetText(StateContainer state) {
            return Text;
        }

        public virtual bool IsAvailable(StateContainer state) {
            if (!Conditions.TrueForAll(g => g.CheckCondition(state))) return false;
            return true;
        }

        public virtual INode GetNextNode(StateContainer state) {
            return Next;
        }
    }
}
