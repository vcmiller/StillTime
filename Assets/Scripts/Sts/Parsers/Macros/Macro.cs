#nullable enable

using System.Collections.Generic;

namespace StillTime.Sts.Parsers.Macros {
    public class Macro {
        public string Identifier { get; }
        private readonly MacroParameters _parameters;
        private readonly List<ISubMacro> _subMacros;

        public Macro(string identifier, MacroParameters parameters, List<ISubMacro> subMacros) {
            Identifier = identifier;
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
}
