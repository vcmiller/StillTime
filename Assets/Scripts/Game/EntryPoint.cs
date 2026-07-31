using System;
using System.Collections.Generic;
using System.IO;
using Commands;
using Cysharp.Threading.Tasks;
using Infohazard.Core;
using Newtonsoft.Json;
using Nodes;
using Parsers;
using UnityEngine;
using UnityEngine.Networking;

namespace Game {
    public class EntryPoint : MonoBehaviour {
        public TextAsset _script;
        public string _scriptUrl = "https://vcmiller.github.io/StillTime/data/GameScript.txt";
        public GameRunner _gameRunner;
        public bool _editorUsesLocalScript = true;

        public PassiveTimer _saveGameTimer;

        private const string SaveGameKey = "StillTime.SaveData";

        private void Start() {
            if (Application.isEditor && _editorUsesLocalScript) {
                LoadFromScriptText(_script.text);
            } else {
                LoadFromWebAsync().Forget();
            }
        }

        private async UniTask LoadFromWebAsync() {
            using DownloadHandlerBuffer buffer = new();
            using UnityWebRequest request = new(new Uri(_scriptUrl), "GET");
            request.downloadHandler = buffer;

            try {
                await request.SendWebRequest();
                using StreamReader reader = new(new MemoryStream(buffer.data));
                LoadFromScriptText(reader.ReadToEnd());
                Debug.Log("Successfully loaded remote script.");
            } catch (Exception ex) {
                Debug.LogException(ex);
                LoadFromScriptText(_script.text);
            }
        }

        private void LoadFromScriptText(string scriptText) {
            List<Command> commands = ScriptParser.ParseScript(scriptText);
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
