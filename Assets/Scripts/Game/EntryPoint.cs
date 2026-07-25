using System;
using System.Collections.Generic;
using Commands;
using Infohazard.Core;
using Newtonsoft.Json;
using Nodes;
using UnityEngine;

namespace Game {
    public class EntryPoint : MonoBehaviour {
        public TextAsset _script;
        public GameRunner _gameRunner;

        public PassiveTimer _saveGameTimer;

        private const string SaveGameKey = "StillTime.SaveData";

        private void Start() {
            List<Command> commands = CommandParser.ParseScript(_script.text);
            GameGraph graph = GraphBuilder.BuildGraph(commands);

            _gameRunner.LoadGameGraph(graph);

            string saveData = PlayerPrefs.GetString(SaveGameKey, string.Empty);

            if (string.IsNullOrEmpty(saveData)) {
                _gameRunner.StartNewGame();
            } else {
                try {
                    SerializedTraversalState serializedState =
                        JsonConvert.DeserializeObject<SerializedTraversalState>(saveData);
                    _gameRunner.LoadGame(serializedState);
                } catch (Exception ex) {
                    Debug.LogException(ex);
                    _gameRunner.StartNewGame();
                }
            }
            
            _saveGameTimer.Initialize();
        }

        private void OnDisable() {
            SaveGame();
        }

        private void Update() {
            if (_saveGameTimer.TryConsume()) {
                SaveGame();
            }
        }

        private void SaveGame() {
            SerializedTraversalState saveData = _gameRunner.SaveGame();
            string json = JsonConvert.SerializeObject(saveData);
            PlayerPrefs.SetString(SaveGameKey, json);
        }
    }
}