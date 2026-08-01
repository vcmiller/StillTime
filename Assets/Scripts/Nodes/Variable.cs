using System;
using StillTime.Commands;

namespace StillTime.Nodes {
    public class Variable : Resource {
        public VarType Type { get; }
        
        public VarScope Scope { get; }

        public object DefaultValue => Type switch {
            VarType.Int => 0,
            VarType.Bool => false,
            VarType.String => string.Empty,
        };
        
        public Variable(string identifier, VarType type, VarScope scope) : base(identifier) {
            Type = type;
            Scope = scope;
        }

        public bool TryParseValue(string str, out object value) {
            switch (Type) {
                case VarType.Int:
                    bool isInt = int.TryParse(str, out int intValue);
                    value = intValue;
                    return isInt;
                case VarType.Bool:
                    bool isBool = bool.TryParse(str, out bool boolValue);
                    value = boolValue;
                    return isBool;
                case VarType.String:
                    value = str;
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}