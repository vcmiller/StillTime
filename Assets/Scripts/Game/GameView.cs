using System;
using System.Collections.Generic;
using DG.Tweening;
using Infohazard.Core;
using Nodes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class GameView : MonoBehaviour {
        public GameObject _mainPanel;
        public TMP_Text _mainText;
        public Button _mainButton;

        public ChoiceView _choiceViewPrefab;
        public Transform _choiceViewParent;

        public TMP_Text _countdownText;
        public GameObject _countdownObject;

        public Camera _camera;

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
            
            _countdownObject.SetActive(false);
        }

        private void OnDisable() {
            _mainButton.onClick.RemoveListener(HandleButton);
        }

        private void HandleButton() {
            if (_mainText.maxVisibleWords < _mainText.textInfo.wordCount + 1 ||
                _visibleButtonCount < _currentChoices.Count) {
                _mainText.maxVisibleWords = _mainText.textInfo.wordCount + 1;
                for (int i = 0; i < _currentChoices.Count; i++) {
                    _currentChoices[i].transform.localScale = Vector3.one;
                }

                _visibleButtonCount = _currentChoices.Count;
                _mainButton.gameObject.SetActive(_currentChoices.Count == 0);
            } else {
                _mainButtonAction?.Invoke();
            }
        }

        private string AddSpeakerToText(string text, Speaker speaker) {
            if (speaker == null) return text;

            string color = ColorUtility.ToHtmlStringRGB(speaker.Color);
            return $"<color=#{color}>{speaker.Text}</color>\n{text}";
        }

        public void SetSingleText(string text, Speaker speaker, Action next) {
            Clear();

            _mainText.text = AddSpeakerToText(text, speaker);
            _showWordTimer.StartInterval();
            _mainButtonAction = next;
        }

        public void SetChoices(string mainText, Speaker speaker, List<(string text, Action action, bool hasNew)> choices) {
            Clear();

            _mainText.text = AddSpeakerToText(mainText, speaker);
            _showWordTimer.StartInterval();
            foreach ((string text, Action action, bool hasNew) in choices) {
                ChoiceView choiceView = Instantiate(_choiceViewPrefab, _choiceViewParent, false);
                choiceView.Configure(text, action, hasNew);
                choiceView.transform.localScale = Vector3.zero;
                _currentChoices.Add(choiceView);
            }
        }

        public void ShowCountdown(string text) {
            _countdownObject.SetActive(true);
            _countdownText.text = text;
        }

        public void HideCountdown() {
            _countdownObject.SetActive(false);
        }

        public void SetBgColor(Color color, float time) {
            _camera.DOKill();
            if (time == 0) {
                _camera.backgroundColor = color;
            } else {
                _camera.DOColor(color, time);
            }
        }

        private void Update() {
            if (_mainText.isActiveAndEnabled &&
                _mainText.text.Length > 0 && 
                _mainText.maxVisibleWords < _mainText.textInfo.wordCount + 1 &&
                _showWordTimer.TryConsume()) {
                _mainText.maxVisibleWords++;
                _showButtonTimer.StartInterval();
            }

            if (_mainText.maxVisibleWords >= _mainText.textInfo.wordCount + 1 &&
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

        public void Clear(bool hidePanel = false) {
            _choiceViewParent.DestroyChildren();
            _mainButtonAction = null;
            _mainText.text = string.Empty;
            _mainText.maxVisibleWords = 0;
            _mainButton.gameObject.SetActive(!hidePanel);
            _visibleButtonCount = 0;
            _currentChoices.Clear();
            _mainPanel.SetActive(!hidePanel);
        }
    }
}