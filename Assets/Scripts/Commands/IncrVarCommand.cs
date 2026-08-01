using System.Collections.Generic;
using StillTime.Nodes;

namespace StillTime.Commands {
    public class IncrVarCommand : Command, ISequentialCommand {
        public string VarName { get; }

        public int Value { get; }

        public IncrVarCommand(int lineNumber, string line, string varName, int value) : base(lineNumber, line) {
            VarName = varName;
            Value = value;
        }

        public void ApplyToSequence(ref ISequentialNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            Variable variable = CommandUtility.GetResource<Variable>(this, VarName, resourceDictionary);

            if (variable.Type != VarType.Int) {
                throw new ParsingException(LineNumber, Line, "Increment is only valid for int variable");
            }

            IncrementVariableNode incrementVariableNode = new(variable, Value);
            createdNodes.Add(incrementVariableNode);
            nextNode.Next = incrementVariableNode;
            nextNode = incrementVariableNode;
        }
    }
}
