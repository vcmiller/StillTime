#nullable enable

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using StillTime.Sts.Parsers;
using StillTime.Sts.Parsers.Macros;

namespace StillTime.Sts.Resources {
    public class Macro : Resource {
        private readonly MacroParameters _parameters;
        private readonly List<ISubMacro> _subMacros;

        private static Regex _stringInterpRegex = new(@"\$[A-Za-z0-9]+");

        public Macro(string identifier, MacroParameters parameters, List<ISubMacro> subMacros) : base(identifier) {
            _parameters = parameters;
            _subMacros = subMacros;
        }

        public void ExpandCall(ParsingState state) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            _parameters.ValidateTokens(tokens);

            List<ParsingState.BufferedLine> expandedLines = new();
            foreach (ISubMacro subMacro in _subMacros) {
                foreach (string line in subMacro.Expand(tokens)) {
                    expandedLines.Add(new ParsingState.BufferedLine(tokens.LineNumber, line));
                }
            }

            state.PrependRange(expandedLines);
        }
    }

    public struct MacroLine {
        public string? Template { get; }
        public Macro? SubMacro { get; }

        public MacroLine(string template) {
            Template = template;
            SubMacro = null;
        }

        public MacroLine(Macro subMacro) {
            SubMacro = subMacro;
            Template = null;
        }
    }
}
