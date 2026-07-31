using System;
using JetBrains.Annotations;

namespace Parsers {
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    [MeansImplicitUse]
    public class CustomCommandParserAttribute : Attribute {
        public string CommandName { get; }

        public CustomCommandParserAttribute(string commandName) {
            CommandName = commandName;
        }
    }
}
