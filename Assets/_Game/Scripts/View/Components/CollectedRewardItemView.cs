using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace WheelOfFortune.View.Components
{
    public sealed class CollectedRewardItemView : MonoBehaviour
    {
        private const string DEFAULT_AMOUNT_TEXT = "0";
        private const float DEFAULT_AMOUNT_TWEEN_DURATION = 0.75f;

        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _amountText;

        private Tween _amountTween;
        private int _displayedAmount;

        public void Initialize(Sprite icon, int amount)
        {
            KillAmountTween();

            if (_iconImage != null)
            {
                _iconImage.sprite = icon;
            }

            _displayedAmount = amount;
            SetAmountText(amount);
        }

        public void UpdateAmount(int amount)
        {
            if (_amountText == null)
            {
                return;
            }

            KillAmountTween();

            int startValue = _displayedAmount;
            int endValue = Mathf.Max(0, amount);

            _amountTween = DOVirtual.Int(startValue, endValue, DEFAULT_AMOUNT_TWEEN_DURATION, SetAmountText);
            _displayedAmount = endValue;
        }

        public void ResetView()
        {
            KillAmountTween();
            _displayedAmount = 0;

            if (_iconImage)
            {
                _iconImage.sprite = null;
            }

            if (_amountText)
            {
                _amountText.text = DEFAULT_AMOUNT_TEXT;
            }
        }

        private void SetAmountText(int amount)
        {
            if (!_amountText)
            {
                return;
            }

            _amountText.text = amount.ToString();
        }

        private void KillAmountTween()
        {
            if (_amountTween == null)
            {
                return;
            }

            if (_amountTween.IsActive())
            {
                _amountTween.Kill();
            }

            _amountTween = null;
        }
    }
}
