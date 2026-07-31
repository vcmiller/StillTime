using System.Collections.Generic;
using Nodes;

namespace Commands {
    public class SetVarCommand : Command, ISequentialCommand {
        public string VarName { get; }
        public string Value { get; }

        public SetVarCommand(int lineNumber, string line, string varName, string value) : base(lineNumber, line) {
            VarName = varName;
            Value = value;
        }

        public void ApplyToSequence(ref ISingleNextNode nextNode,
                                    Dictionary<string, Resource> resourceDictionary,
                                    Dictionary<string, INode> nodeDictionary,
                                    List<INode> createdNodes) {
            Variable variable = CommandUtility.GetResource<Variable>(this, VarName, resourceDictionary);
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
