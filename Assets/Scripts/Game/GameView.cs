using System;
using System.Collections.Generic;
using DG.Tweening;
using Infohazard.Core;
using StillTime.Nodes;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace StillTime.Game {
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
        private TMP_TextInfo _textInfo;
        private bool _autoAdvance;

        private void OnEnable() {
            Clear(true);

            _mainButton.onClick.RemoveListener(HandleButton);
            _mainButton.onClick.AddListener(HandleButton);

            _showWordTimer.Initialize();
            _showButtonTimer.Initialize();

            HideCountdown();
        }

        private void OnDisable() {
            _mainButton.onClick.RemoveListener(HandleButton);
        }

        private void SetText(string text) {
            _mainText.text = text;
            _textInfo = _mainText.GetTextInfo(text);
        }

        private void SkipAnimation() {
            _mainText.maxVisibleWords = _textInfo?.wordCount ?? 0 + 1;
            for (int i = 0; i < _currentChoices.Count; i++) {
                _currentChoices[i].transform.localScale = Vector3.one;
            }

            _visibleButtonCount = _currentChoices.Count;
            _mainButton.gameObject.SetActive(_currentChoices.Count == 0);
        }

        private void HandleButton() {
            if (_mainText.maxVisibleWords < _textInfo?.wordCount + 1 ||
                _visibleButtonCount < _currentChoices.Count) {
                SkipAnimation();
            } else {
                _mainButtonAction?.Invoke();
            }
        }

        private string AddSpeakerToText(string text, Speaker speaker) {
            if (speaker == null) return text;

            string color = ColorUtility.ToHtmlStringRGB(speaker.Color);
            return $"<color=#{color}>{speaker.Text}</color>\n{text}";
        }

        public void SetSingleText(string text, Speaker speaker, Action next, bool skipAnimation, bool autoAdvance) {
            Clear();

            SetText(AddSpeakerToText(text, speaker));
            _mainButtonAction = next;

            if (skipAnimation) {
                SkipAnimation();
            }

            _autoAdvance = autoAdvance;
            _showWordTimer.StartInterval();
        }

        public void SetChoices(string mainText, Speaker speaker, List<(string text, Action action, bool hasNew)> choices, bool skipAnimation) {
            Clear();

            SetText(AddSpeakerToText(mainText, speaker));
            foreach ((string text, Action action, bool hasNew) in choices) {
                ChoiceView choiceView = Instantiate(_choiceViewPrefab, _choiceViewParent, false);
                choiceView.Configure(text, action, hasNew);
                choiceView.transform.localScale = Vector3.zero;
                _currentChoices.Add(choiceView);
            }

            if (skipAnimation) {
                SkipAnimation();
            } else {
                _showWordTimer.StartInterval();
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
                _textInfo != null &&
                _mainText.maxVisibleWords < _textInfo.wordCount + 1 &&
                _showWordTimer.TryConsume()) {
                _mainText.maxVisibleWords++;
                _showButtonTimer.StartInterval();
            }

            if (_textInfo != null &&
                _mainText.maxVisibleWords >= _textInfo.wordCount + 1 &&
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

            if (_textInfo != null &&
                _mainText.maxVisibleWords >= _textInfo.wordCount + 1 &&
                _currentChoices.Count == 0 &&
                _autoAdvance &&
                _mainButtonAction != null &&
                _showWordTimer.TryConsume()) {
                _mainButtonAction();
            }
        }

        public void Clear(bool hidePanel = false) {
            _choiceViewParent.DestroyChildren();
            _mainButtonAction = null;
            SetText(string.Empty);
            _mainText.maxVisibleWords = 0;
            _mainButton.gameObject.SetActive(!hidePanel);
            _visibleButtonCount = 0;
            _currentChoices.Clear();
            _mainPanel.SetActive(!hidePanel);
        }
    }
}
