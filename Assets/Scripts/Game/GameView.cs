using System;
using System.Collections.Generic;
using Infohazard.Core;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class GameView : MonoBehaviour {
        public TMP_Text _mainText;
        public Button _mainButton;

        public ChoiceView _choiceViewPrefab;
        public Transform _choiceViewParent;

        private Action _mainButtonAction;

        private void OnEnable() {
            _mainButton.onClick.RemoveListener(HandleButton);
            _mainButton.onClick.AddListener(HandleButton);
        }

        private void OnDisable() {
            _mainButton.onClick.RemoveListener(HandleButton);
        }

        private void HandleButton() {
            _mainButtonAction?.Invoke();
        }

        public void SetSingleText(string text, Action next) {
            Clear();

            _mainText.text = text;
            _mainButtonAction = next;
            _mainButton.gameObject.SetActive(true);
        }

        public void SetChoices(string mainText, List<(string text, Action action)> choices) {
            Clear();

            _mainText.text = mainText;
            foreach ((string text, Action action) in choices) {
                ChoiceView choiceView = Instantiate(_choiceViewPrefab, _choiceViewParent, false);
                choiceView.Configure(text, action);
            }
        }

        public void Clear() {
            _choiceViewParent.DestroyChildren();
            _mainButtonAction = null;
            _mainText.text = string.Empty;
            _mainButton.gameObject.SetActive(false);
        }
    }
}