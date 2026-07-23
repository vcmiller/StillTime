using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game {
    public class ChoiceView: MonoBehaviour {
        public Button _button;
        public TMP_Text _label;
        
        public void Configure(string text, Action action) {
            _label.text = text;
            _button.onClick.AddListener(() => action());
        }
    }
}