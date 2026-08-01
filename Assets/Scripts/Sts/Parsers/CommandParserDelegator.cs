using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using StillTime.Sts.Commands;

namespace StillTime.Sts.Parsers {
    public static class CommandParserDelegator {
        private static readonly Dictionary<string, ICommandParser> AllCommandParsers = new();

        static CommandParserDelegator() {
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                foreach (Type type in assembly.GetTypes()) {
                    CustomCommandParserAttribute[] attributes =
                        type.GetCustomAttributes<CustomCommandParserAttribute>().ToArray();

                    if (attributes.Length == 0) continue;

                    ICommandParser commandParser;
                    try {
                        object instance = Activator.CreateInstance(type);
                        if (instance is not ICommandParser tempParser) {
                            throw new Exception($"Type {type} is not assignable to ICommandParser");
                        }

                        commandParser = tempParser;
                    } catch (Exception ex) {
                        StsLibrary.Logger.LogError(ex, "Exception occurred instantiated command parser {Type}", type);
                        continue;
                    }

                    foreach (CustomCommandParserAttribute attribute in attributes) {
                        AllCommandParsers[attribute.CommandName] = commandParser;
                    }
                }
            }
        }

        public static Command ParseCommand(string[] lines, ref int lineNumber) {
            string line = lines[lineNumber];

            ReadOnlySpan<char> actualSpan = ParsingUtility.GetActualSpanFromLine(line);
            if (actualSpan.IsEmpty) {
                lineNumber++;
                return null;
            }

            ParsingUtility.ReadCommandLine(
                line,
                lineNumber,
                out string cmd,
                out string text,
                out string[] args,
                out bool isTextContinued);

            if (!AllCommandParsers.TryGetValue(cmd, out ICommandParser parser)) {
                throw new ParsingException(lineNumber, line, $"No parser found for command '{cmd}'");
            }

            int lineNumberBefore = lineNumber;
            Command result = parser.ParseCommand(lines, ref lineNumber, cmd, args, text, isTextContinued);

            if (lineNumber <= lineNumberBefore) {
                lineNumber = lineNumberBefore + 1;
            }

            return result;
        }
    }
}
