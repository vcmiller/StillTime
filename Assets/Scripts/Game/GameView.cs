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

        public PassiveTimer _showWordTimer;
        public PassiveTimer _showButtonTimer;

        private Action _mainButtonAction;
        private int _visibleButtonCount;
        private readonly List<ChoiceView> _currentChoices = new();

        private void OnEnable() {
            _mainButton.onClick.RemoveListener(HandleButton);
            _mainButton.onClick.AddListener(HandleButton);
            
            _showWordTimer.Initialize();
            _showButtonTimer.Initialize();
        }

        private void OnDisable() {
            _mainButton.onClick.RemoveListener(HandleButton);
        }

        private void HandleButton() {
            if (_mainText.maxVisibleWords < _mainText.textInfo.wordCount ||
                _visibleButtonCount < _currentChoices.Count) {
                _mainText.maxVisibleWords = _mainText.textInfo.wordCount;
                for (int i = 0; i < _currentChoices.Count; i++) {
                    _currentChoices[i].transform.localScale = Vector3.one;
                }

                _visibleButtonCount = _currentChoices.Count;
                _mainButton.gameObject.SetActive(_currentChoices.Count == 0);
            } else {
                _mainButtonAction?.Invoke();
            }
        }

        public void SetSingleText(string text, Action next) {
            Clear();

            _mainText.text = text;
            _showWordTimer.StartInterval();
            _mainButtonAction = next;
        }

        public void SetChoices(string mainText, List<(string text, Action action)> choices) {
            Clear();

            _mainText.text = mainText;
            _showWordTimer.StartInterval();
            foreach ((string text, Action action) in choices) {
                ChoiceView choiceView = Instantiate(_choiceViewPrefab, _choiceViewParent, false);
                choiceView.Configure(text, action);
                choiceView.transform.localScale = Vector3.zero;
                _currentChoices.Add(choiceView);
            }
        }

        private void Update() {
            if (_mainText.isActiveAndEnabled &&
                _mainText.text.Length > 0 && 
                _mainText.maxVisibleWords < _mainText.textInfo.wordCount &&
                _showWordTimer.TryConsume()) {
                _mainText.maxVisibleWords++;
                _showButtonTimer.StartInterval();
            }

            if (_mainText.maxVisibleWords >= _mainText.textInfo.wordCount &&
                _currentChoices.Count > 0 &&
                _visibleButtonCount < _currentChoices.Count) {

                ChoiceView currentButton = _currentChoices[_visibleButtonCount];
                float ratio = _showButtonTimer.RatioSinceIntervalStart;
                currentButton.transform.localScale = Vector3.one * ratio;

                if (ratio == 1) {
                    _showButtonTimer.StartInterval();
                    _visibleButtonCount++;

                    if (_visibleButtonCount == _currentChoices.Count) {
                        _mainButton.gameObject.SetActive(false);
                    }
                }
            }
        }

        public void Clear() {
            _choiceViewParent.DestroyChildren();
            _mainButtonAction = null;
            _mainText.text = string.Empty;
            _mainText.maxVisibleWords = 0;
            _mainButton.gameObject.SetActive(true);
            _visibleButtonCount = 0;
            _currentChoices.Clear();
        }
    }
}