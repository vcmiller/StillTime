using System;
using Commands;
using Game;

namespace Nodes {
    public class IncrementVariableNode : Node, ISingleNextNode {
        public INode Next { get; set; }
    
        public Variable Variable { get; }
        
        public int Increment { get; }
        
        public IncrementVariableNode(Variable variable, int increment) {
            if (variable.Type != VarType.Int) {
                throw new Exception("Increment is only valid for int variable.");
            }
            
            Variable = variable;
            Increment = increment;
        }

        public override void ApplyToState(ref MutableTraversalState state) {
            base.ApplyToState(ref state);
            int previousValue = state.GetVariableValue<int>(Variable);
            state.SetVariableValue(Variable, previousValue + Increment);
        }
    }
}