using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class SetVarCommand : Command {
        public string VarName { get; }
        public string Value { get; }

        public SetVarCommand(int lineNumber, string line, string varName, string value) : base(lineNumber, line) {
            VarName = varName;
            Value = value;
        }

        public override void ApplyToSequence(ref ISingleNextNode nextNode,
                                             IReadOnlyDictionary<string, Resource> resources,
                                             IReadOnlyDictionary<string, INode> nodeDictionary,
                                             List<INode> createdNodes) {
            
            Variable variable = CommandUtility.GetResource<Variable>(this, VarName, resources);
            if (!variable.TryParseValue(Value, out object varValue)) {
                throw new ParsingException(LineNumber, Line, $"Invalid value {Value} for var type {variable.Type}");
            }

            SetVariableNode setVariableNode = new(variable, varValue);
            createdNodes.Add(setVariableNode);
            nextNode.Next = setVariableNode;
            nextNode = setVariableNode;
        }
    }
}