using System;
using StillTime.Sts.Utility;

namespace StillTime.Sts.Resources {
    public class Variable : Resource {
        public StsValueType Type { get; }

        public string ScopeId { get; }

        public StsValue DefaultValue => Type switch {
            StsValueType.Number => new StsValue(0),
            StsValueType.Color => new StsValue(default(StsColor)),
            StsValueType.String => new StsValue(string.Empty),
            StsValueType.Bool => new StsValue(false),
            StsValueType.None => default,
            _ => throw new ArgumentOutOfRangeException(),
        };

        public Variable(string identifier, StsValueType type, string scopeId) : base(identifier) {
            Type = type;
            ScopeId = scopeId;
        }

        public bool TryParseValue(string str, out StsValue value) {
            switch (Type) {
                case StsValueType.Number:
                    bool isInt = decimal.TryParse(str, out decimal decValue);
                    value = new StsValue(decValue);
                    return isInt;
                case StsValueType.Color:
                    bool isColor = StsColor.TryParseHex(str, out StsColor colorValue);
                    value = new StsValue(colorValue);
                    return isColor;
                case StsValueType.Bool:
                    bool isBool = bool.TryParse(str, out bool boolValue);
                    value = new StsValue(boolValue);
                    return isBool;
                case StsValueType.String:
                    value = new StsValue(str);
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
