using System.Collections.Generic;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Commands.Utility;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Commands {
    public class SetVarCommand : Command, ISequentialCommand {
        public string VarName { get; }
        public string Value { get; }

        public SetVarCommand(int lineNumber, string line, string varName, string value) : base(lineNumber, line) {
            VarName = varName;
            Value = value;
        }

        public void ApplyToSequence(NodeSequenceBuilder builder, GraphData graphData) {
            Variable variable = graphData.GetResource<Variable>(this, VarName);
            if (!StsValue.TryParse(Value, variable.Type, out StsValue varValue)) {
                throw new ParsingException(LineNumber, Line, $"Invalid value {Value} for var type {variable.Type}");
            }

            SetVariableNode setVariableNode = new(variable, varValue);
            builder.Append(setVariableNode);
        }
    }
}
