#nullable enable

using System;
using System.Collections.Generic;
using System.Linq;
using StillTime.Sts.Commands;
using StillTime.Sts.Commands.Interfaces;
using StillTime.Sts.Parsers.Macros;
using StillTime.Sts.Resources;

namespace StillTime.Sts.Parsers {
    [CustomCommandParser("macro")]
    public class MacroCommandParser : ICommandParser {

        public void ParseCommand(ParsingState state, List<ICommand> commands) {
            LineTokens tokens = Tokenizer.TokenizeAndAdvance(state);
            Tokenizer.ValidateTokens(tokens, 1, 1000, false, true);

            string identifier = tokens.Arguments[0];
            MacroParameters macroParameters = ParseMacroParameters(tokens);
            List<ISubMacro> subMacros = new();

            ParseSubMacros(state, macroParameters, subMacros);

            Macro macro = new(identifier, macroParameters, subMacros);
            state.Macros.Add(identifier, macro);
        }

        private static MacroParameters ParseMacroParameters(LineTokens tokens) {
            string[] parameters = tokens.Arguments[1..];
            List<MacroParameter> normalParams = new();
            List<MacroParameter> optionalParams = new();
            MacroParameter? varArgsParam = null;
            MacroParameter? textParam = null;

            for (int i = 0; i < parameters.Length; i++) {
                string param = parameters[i];
                string paramName;
                MacroParameterType paramType;
                string? defaultValue;

                if (param.EndsWith("...")) {
                    paramName = param[..^3];
                    paramType = MacroParameterType.VarArg;
                    defaultValue = string.Empty;
                } else if (param.EndsWith("?")) {
                    paramName = param[..^1];
                    paramType = MacroParameterType.Regular;
                    defaultValue = string.Empty;
                } else if (param.Contains('=')) {
                    int index = param.IndexOf('=');
                    paramName = param[..index].Trim();
                    paramType = MacroParameterType.Regular;
                    defaultValue = param[(index + 1)..];
                } else {
                    paramName = param;
                    paramType = MacroParameterType.Regular;
                    defaultValue = null;
                }

                if (!Tokenizer.IsValidName(paramName)) {
                    throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                               $"Invalid macro parameter name '{paramName}'");
                }

                if (paramType == MacroParameterType.VarArg) {
                    if (i == parameters.Length - 1) {
                        varArgsParam = new MacroParameter(paramName, MacroParameterType.VarArg, defaultValue);
                    } else {
                        throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                                   $"VarArg parameter {paramName} only allowed as last parameter");
                    }
                } else {
                    if (defaultValue != null) {
                        optionalParams.Add(new MacroParameter(paramName, MacroParameterType.Regular, defaultValue));
                    } else {
                        if (optionalParams.Count == 0) {
                            normalParams.Add(new MacroParameter(paramName, MacroParameterType.Regular));
                        } else {
                            throw new ParsingException(
                                tokens.LineNumber,
                                tokens.OriginalLine,
                                $"Non-optional parameter {paramName} not allowed after optional parameters.");
                        }
                    }
                }
            }

            if (!string.IsNullOrEmpty(tokens.Text)) {
                if (Tokenizer.IsValidName(tokens.Text)) {
                    textParam = new MacroParameter(tokens.Text, MacroParameterType.Text, string.Empty);
                } else {
                    throw new ParsingException(tokens.LineNumber, tokens.OriginalLine,
                                               $"Invalid macro parameter name '{tokens.Text}'");
                }
            }

            MacroParameters macroParameters = new(normalParams, optionalParams, varArgsParam, textParam);
            return macroParameters;
        }

        private static void ParseSubMacros(
            ParsingState state,
            MacroParameters macroParameters,
            List<ISubMacro> subMacros) {

            while (!state.IsEnded) {
                string line = state.CurrentLine!;
                ReadOnlySpan<char> actualRange = Tokenizer.GetActualSpanFromLine(line);
                if (actualRange.IsEmpty) {
                    state.MoveNext();
                    continue;
                }

                ISubMacro? subMacro = ParseSubMacro(state, actualRange, macroParameters);
                if (subMacro == null) break;

                subMacros.Add(subMacro);
            }
        }

        private static ISubMacro? ParseSubMacro(
            ParsingState state,
            ReadOnlySpan<char> actualRange,
            MacroParameters macroParameters) {

            macroParameters.ValidateMacroLine(state.LineNumber, state.CurrentLine!);

            if (!actualRange.StartsWith("!")) {
                string line = state.MoveNext()!;

                return new RegularLineSubMacro(macroParameters, line);
            } else {
                LineTokens subTokens = Tokenizer.TokenizeAndAdvance(state);
                switch (subTokens.Command) {
                    case "!end":
                        Tokenizer.ValidateTokens(subTokens, 0, 0, false);
                        return null;
                    case "!if":
                        Tokenizer.ValidateTokens(subTokens, 1, 100, false);
                        return ParseIfStatement(subTokens, state, macroParameters);
                    default:
                        throw new ParsingException(subTokens.LineNumber, subTokens.OriginalLine,
                                                   $"Unrecognized macro command '{subTokens.Command}'");
                }
            }
        }

        private static IfStatementSubMacro ParseIfStatement(
            LineTokens ifStartTokens,
            ParsingState state,
            MacroParameters macroParameters) {

            MacroIf ifSection = ParseIf(ifStartTokens, state, macroParameters);
            List<MacroIf> elseIfs = new();
            List<ISubMacro> elseSection = new();

            while (!state.IsEnded) {
                string line = state.CurrentLine!;
                ReadOnlySpan<char> actualRange = Tokenizer.GetActualSpanFromLine(line);
                if (actualRange.IsEmpty) {
                    state.MoveNext();
                    continue;
                }

                macroParameters.ValidateMacroLine(state.LineNumber, state.CurrentLine!);
                LineTokens subTokens = Tokenizer.TokenizeAndAdvance(state);
                bool isEnd = false;
                switch (subTokens.Command) {
                    case "!end":
                        Tokenizer.ValidateTokens(subTokens, 0, 0, false);
                        isEnd = true;
                        break;
                    case "!elif" when elseSection.Count == 0:
                        Tokenizer.ValidateTokens(subTokens, 1, 100, false);
                        elseIfs.Add(ParseIf(subTokens, state, macroParameters));
                        break;
                    case "!else" when elseSection.Count == 0:
                        Tokenizer.ValidateTokens(subTokens, 0, 0, false);
                        ParseSubMacros(state, macroParameters, elseSection);
                        break;
                    default:
                        throw new ParsingException(subTokens.LineNumber, subTokens.OriginalLine,
                                                   $"Unexpected macro command '{subTokens.Command}'");
                }

                if (isEnd) break;
            }

            return new IfStatementSubMacro(ifSection, elseIfs, elseSection);
        }

        private static MacroIf ParseIf(
            LineTokens tokens,
            ParsingState state,
            MacroParameters macroParameters) {

            string[] conditions = tokens.Arguments;
            foreach (string condition in conditions) {
                if (macroParameters.GetMacroParameter(condition) == null) {
                    throw new ParsingException(tokens.LineNumber, tokens.Text,
                                               $"Unrecognized macro parameter '{condition}'");
                }
            }

            List<ISubMacro> ifSection = new();
            ParseSubMacros(state, macroParameters, ifSection);
            return new MacroIf(macroParameters, conditions.ToList(), ifSection);
        }
    }
}
