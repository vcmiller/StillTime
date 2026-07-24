using System;
using System.Collections.Generic;
using System.Linq;
using Commands;
using Nodes;
using UnityEngine;

namespace Game {
    public class ScriptRunner : MonoBehaviour {
        public TextAsset _script;
        public GameRunner _gameRunner;

        private void OnEnable() {
            List<Command> commands = ScriptParser.ParseScript(_script.text);
            GameGraph graph = GraphBuilder.BuildGraph(commands);

            _gameRunner.LoadGameGraph(graph);
            _gameRunner.StartNewGame();
        }

        private void OnDisable() {
            
        }
    }
}