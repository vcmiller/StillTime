using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StillTime.Game {
    public class SettingsMenu : MonoBehaviour {
        public Toggle _skipToggle;
        public Toggle _skipSeenToggle;
        public GameRunner _gameRunner;
        public TMP_InputField _jumpInput;

        private void OnEnable() {
            _skipToggle.isOn = _gameRunner.SkipAnimations;
            _skipSeenToggle.isOn = _gameRunner.SkipSeenDialogue;
        }

        public void ToggleAnimations(bool value) {
            _gameRunner.SkipAnimations = value;
        }

        public void ToggleSkipSeen(bool value) {
            _gameRunner.SkipSeenDialogue = value;
        }

        public void ResetGame() {
            _gameRunner.ClearGameState();
            _gameRunner.StartNewGame();
        }

        public void DoJump() {
            string text = _jumpInput.text;
            if (string.IsNullOrEmpty(text)) return;

            SerializedTraversalState prevState = _gameRunner.SaveGame();
            try {
                SerializedTraversalState modState = prevState.Clone();
                modState.CurrentNode = text;
                _gameRunner.ClearGameState();
                _gameRunner.LoadGame(modState);
            } catch (Exception ex) {
                Debug.LogException(ex);
                _gameRunner.LoadGame(prevState);
            }
        }
    }
}
