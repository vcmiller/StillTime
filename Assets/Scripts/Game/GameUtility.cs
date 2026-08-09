using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using StillTime.Sts.Nodes;
using StillTime.Sts.Resources;
using StillTime.Sts.Runtime;
using StillTime.Sts.Runtime.Components;
using UnityEngine;

namespace StillTime.Game {
    public static class GameUtility {
        private static readonly Regex StringInterpRegex = new(@"\{[0-9a-zA-Z_]*\}");

        public static string DoStringInterpolation(string str, GameGraph gameGraph, StateContainer state) {
            try {
                string currentString = str;
                while (StringInterpRegex.Match(currentString) is { Success: true } match) {
                    string varName = currentString.Substring(match.Index + 1, match.Length - 2);
                    if (!gameGraph.ResourcesByIdentifier.TryGetValue(varName, out Resource resource)) {
                        Debug.LogError($"Invalid variable identifier: {varName} in interpolated string {str}");
                        return currentString;
                    }

                    if (resource is not Variable variable) {
                        Debug.LogError($"Invalid variable identifier: {varName} in interpolated string {str}");
                        return currentString;
                    }

                    string varValue = state.GetOrCreate<VariablesComponent>().GetVariableValue(variable).ToString();

                    currentString = currentString.Replace(match.Value, varValue);
                }

                return currentString;
            } catch (Exception ex) {
                Debug.LogException(ex);
                return str;
            }
        }
    }
}
