using System;
using Commands;
using Game;

namespace Nodes {
    public class UnlockNode : Node, ISingleNextNode {
        public INode Next { get; set; }
    
        public Variable Variable { get; }
        
        public UnlockNode(Variable variable) {
            if (variable.Type != VarType.Bool) {
                throw new ArgumentException($"Variable {variable} is not of correct type for unlock node.");
            }
            
            Variable = variable;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            state.SetVariableValue(Variable, true);
        }
    }
}