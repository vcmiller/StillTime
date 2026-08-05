using System;
using System.Globalization;

namespace StillTime.Sts.Utility {
    public readonly struct StsValue {
        public StsValueType ValueType { get; }

        private readonly decimal _numberValue;
        private readonly StsColor _colorValue;
        private readonly string _stringValue;
        private readonly bool _boolValue;

        public decimal NumberValue =>
            ValueType == StsValueType.Number
                ? _numberValue
                : throw new InvalidOperationException($"Trying to get number value from value of type {ValueType}");

        public StsColor ColorValue =>
            ValueType == StsValueType.Color
                ? _colorValue
                : throw new InvalidOperationException($"Trying to get color value from value of type {ValueType}");

        public string StringValue =>
            ValueType == StsValueType.String
                ? _stringValue
                : throw new InvalidOperationException($"Trying to get string value from value of type {ValueType}");

        public bool BoolValue =>
            ValueType == StsValueType.Bool
                ? _boolValue
                : throw new InvalidOperationException($"Trying to get bool value from value of type {ValueType}");

        public StsValue(decimal numberValue) {
            ValueType = StsValueType.Number;
            _colorValue = default;
            _stringValue = null;
            _boolValue = false;
            _numberValue = numberValue;
        }

        public StsValue(StsColor colorValue) {
            ValueType = StsValueType.Color;
            _numberValue = 0;
            _stringValue = null;
            _boolValue = false;
            _colorValue = colorValue;
        }

        public StsValue(string stringValue) {
            ValueType = StsValueType.String;
            _numberValue = 0;
            _colorValue = default;
            _boolValue = false;
            _stringValue = stringValue;
        }

        public StsValue(bool boolValue) {
            ValueType = StsValueType.Bool;
            _numberValue = 0;
            _colorValue = default;
            _stringValue = null;
            _boolValue = boolValue;
        }

        public override string ToString() {
            return ValueType switch {
                StsValueType.None => "null",
                StsValueType.Number => _numberValue.ToString(CultureInfo.CurrentCulture),
                StsValueType.Color => _colorValue.ToHexString(),
                StsValueType.String => _stringValue,
                StsValueType.Bool => _boolValue.ToString(),
                _ => throw new ArgumentOutOfRangeException(),
            };
        }

        public bool ToBool() {
            return ValueType switch {
                StsValueType.None => false,
                StsValueType.Number => _numberValue != 0,
                StsValueType.Color => _colorValue != default,
                StsValueType.String => !string.IsNullOrEmpty(_stringValue),
                StsValueType.Bool => _boolValue,
                _ => throw new ArgumentOutOfRangeException(),
            };
        }
    }

    public enum StsValueType {
        None,
        Number,
        Color,
        String,
        Bool,
    }
}
