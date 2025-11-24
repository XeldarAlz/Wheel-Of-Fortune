using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data.Rewards;

namespace WheelOfFortune.View.Components
{
    public sealed class WheelSliceView : MonoBehaviour
    {
        private const float STAR_VISIBLE_ALPHA = 1.0f;
        private const float STAR_HIDDEN_ALPHA = 0.0f;
        private const float STAR_FADE_DURATION = 0.1f;
        private const float STAR_ROTATION_DURATION = 2.0f;
        private const float FULL_ROTATION_DEGREES = 180.0f;
        private const string BOMB_TEXT = "BOMB";

        [SerializeField] private Image _iconImage;
        [SerializeField] private TMP_Text _rewardValue;
        [SerializeField] private Image _starEffectImage;

        private Tween _starRotationTween;
        private Tween _starAlphaTween;

        private void Awake()
        {
            InitializeStarEffect();
        }

        public void Render(WheelSliceConfig sliceConfig)
        {
            if (_iconImage)
            {
                _iconImage.enabled = sliceConfig.Icon;
                _iconImage.sprite = sliceConfig.Icon;
            }

            if (_rewardValue)
            {
                _rewardValue.text = GetRewardDisplayText(sliceConfig);
            }
        }

        public void ShowStarEffect()
        {
            if (!_starEffectImage)
            {
                return;
            }

            TweenStarAlpha(STAR_VISIBLE_ALPHA, false);
            StartStarRotation();
        }

        public void HideStarEffect()
        {
            if (!_starEffectImage)
            {
                return;
            }

            TweenStarAlpha(STAR_HIDDEN_ALPHA, true);
        }

        private string GetRewardDisplayText(WheelSliceConfig sliceConfig)
        {
            switch (sliceConfig.RewardType)
            {
                case WheelRewardType.Bomb:
                    return BOMB_TEXT;

                case WheelRewardType.Currency:
                    return sliceConfig.RewardValue.ToString();

                case WheelRewardType.Item:
                    return sliceConfig.RewardValue.ToString();

                default:
                    return sliceConfig.RewardValue.ToString();
            }
        }

        private void InitializeStarEffect()
        {
            SetStarAlpha(STAR_HIDDEN_ALPHA);
        }

        private void SetStarAlpha(float alpha)
        {
            if (!_starEffectImage)
            {
                return;
            }

            Color starColor = _starEffectImage.color;
            starColor.a = alpha;
            _starEffectImage.color = starColor;
        }

        private void TweenStarAlpha(float targetAlpha, bool stopRotationOnComplete)
        {
            KillStarAlphaTween();

            if (!_starEffectImage)
            {
                return;
            }

            float startAlpha = _starEffectImage.color.a;

            if (Mathf.Approximately(startAlpha, targetAlpha))
            {
                if (stopRotationOnComplete)
                {
                    StopStarRotation();
                }

                return;
            }

            _starAlphaTween = DOVirtual.Float(startAlpha, targetAlpha, STAR_FADE_DURATION, SetStarAlpha);

            if (stopRotationOnComplete)
            {
                _starAlphaTween.OnComplete(() =>
                {
                    StopStarRotation();
                    _starAlphaTween = null;
                });
            }
            else
            {
                _starAlphaTween.OnComplete(() => _starAlphaTween = null);
            }
        }

        private void StartStarRotation()
        {
            if (!_starEffectImage)
            {
                return;
            }

            StopStarRotation();

            _starRotationTween = _starEffectImage.transform
                .DORotate(new Vector3(0.0f, 0.0f, FULL_ROTATION_DEGREES), STAR_ROTATION_DURATION, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental);
        }

        private void StopStarRotation()
        {
            if (_starRotationTween == null)
            {
                return;
            }

            if (_starRotationTween.IsActive())
            {
                _starRotationTween.Kill();
            }

            _starRotationTween = null;

            if (_starEffectImage)
            {
                _starEffectImage.transform.localRotation = Quaternion.identity;
            }
        }

        private void KillStarAlphaTween()
        {
            if (_starAlphaTween == null)
            {
                return;
            }

            if (_starAlphaTween.IsActive())
            {
                _starAlphaTween.Kill();
            }

            _starAlphaTween = null;
        }
        
        private void OnDestroy()
        {
            StopStarRotation();
            KillStarAlphaTween();
        }
    }
}
