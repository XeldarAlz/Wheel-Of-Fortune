using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.View.Components
{
    public sealed class BombView : MonoBehaviour, IBombView
    {
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private Button _reviveButton;
        [SerializeField] private TMP_Text _reviveCostText;
        
        public event Action GiveUpRequested;
        public event Action ReviveRequested;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_giveUpButton == null)
            {
                _giveUpButton = FindButtonByName("GiveUp");
            }

            if (_reviveButton == null)
            {
                _reviveButton = FindButtonByName("Revive");
            }

            if (_reviveCostText == null)
            {
                _reviveCostText = FindTextByName("Revive");
            }
        }

        private Button FindButtonByName(string buttonName)
        {
            Button[] buttons = GetComponentsInChildren<Button>(true);

            for (int index = 0; index < buttons.Length; index++)
            {
                Button button = buttons[index];
                if (button.gameObject.name.Contains(buttonName))
                {
                    return button;
                }
            }

            return null;
        }

        private TMP_Text FindTextByName(string textName)
        {
            TMP_Text[] texts = GetComponentsInChildren<TMP_Text>(true);
            
            for (int index = 0; index < texts.Length; index++)
            {
                TMP_Text text = texts[index];
                if (text.gameObject.name.Contains(textName))
                {
                    return text;
                }
            }

            return null;
        }
#endif

        private void OnEnable()
        {
            SubscribeToButtons();
        }

        private void SubscribeToButtons()
        {
            if (_giveUpButton)
            {
                _giveUpButton.onClick.AddListener(HandleGiveUpClicked);
            }

            if (_reviveButton)
            {
                _reviveButton.onClick.AddListener(HandleReviveClicked);
            }
        }

        public void Show()
        {
        }

        public void Hide()
        {
        }

        public void UpdateReviveButtonState(bool canRevive)
        {
            if (_reviveButton)
            {
                _reviveButton.interactable = canRevive;
            }
        }

        public void UpdateReviveCostText(int cost)
        {
            if (_reviveCostText == null)
            {
                return;
            }

            _reviveCostText.text = $"REVIVE ({cost}$)";
        }

        private void HandleGiveUpClicked()
        {
            GiveUpRequested?.Invoke();
        }

        private void HandleReviveClicked()
        {
            ReviveRequested?.Invoke();
        }

        private void UnsubscribeFromButtons()
        {
            if (_giveUpButton)
            {
                _giveUpButton.onClick.RemoveListener(HandleGiveUpClicked);
            }

            if (_reviveButton)
            {
                _reviveButton.onClick.RemoveListener(HandleReviveClicked);
            }
        }
        
        private void OnDisable()
        {
            UnsubscribeFromButtons();
        }
    }
}
