using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Zone;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;

namespace WheelOfFortune.View.Components
{
    public sealed class WheelView : MonoBehaviour
    {
        private const float DEFAULT_SPRITE_TRANSITION_DURATION = 0.25f;
        private const float HALF_TRANSITION_RATIO = 0.5f;
        private const float ZERO_ALPHA = 0.0f;

        [SerializeField] private TMP_Text _spinText;
        [SerializeField] private TMP_Text _statusText;
        [SerializeField] private Image _wheelBackgroundImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private WheelSliceView[] _sliceViews;
        [SerializeField] private WheelSpinAnimator _wheelSpinAnimator;
        [Header("Collected Rewards")]
        [SerializeField] private GameObject _collectedRewardsLabel;
        [SerializeField] private Transform _collectedRewardsContainer;
        [SerializeField] private CollectedRewardItemView _rewardItemPrefab;
        [Header("Transition Settings")]
        [SerializeField] private float _spriteTransitionDuration = DEFAULT_SPRITE_TRANSITION_DURATION;

        private Tween _wheelTransitionTween;
        private Tween _pointerTransitionTween;
        private Tween _backgroundColorTween;
        private IHapticFeedbackService _hapticService;
        private IAudioService _audioService;

        public event Action SpinRequested;
        public event Action LeaveRequested;

        public GameObject CollectedRewardsLabel => _collectedRewardsLabel;
        public Transform CollectedRewardsContainer => _collectedRewardsContainer;
        public CollectedRewardItemView RewardItemPrefab => _rewardItemPrefab;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!_spinButton)
            {
                _spinButton = FindButtonByName("Spin");
            }

            if (!_leaveButton)
            {
                _leaveButton = FindButtonByName("Leave");
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
#endif

        private void Awake()
        {
            SubscribeToButtons();
        }

        private void Start()
        {
            _hapticService = ServiceLocator.Instance.Get<IHapticFeedbackService>();
            _audioService = ServiceLocator.Instance.Get<IAudioService>();
        }

        private void SubscribeToButtons()
        {
            if (_spinButton)
            {
                _spinButton.onClick.AddListener(HandleSpinClicked);
            }

            if (_leaveButton)
            {
                _leaveButton.onClick.AddListener(HandleLeaveClicked);
            }
        }

        public void RenderSlices(WheelSliceConfig[] sliceConfigs)
        {
            if (_sliceViews == null)
            {
                return;
            }

            int sliceViewCount = _sliceViews.Length;

            for (int index = 0; index < sliceViewCount; index++)
            {
                WheelSliceView sliceView = _sliceViews[index];

                if (!sliceView)
                {
                    continue;
                }

                bool hasSlice = sliceConfigs != null && index < sliceConfigs.Length;
                sliceView.gameObject.SetActive(hasSlice);

                if (hasSlice)
                {
                    sliceView.Render(sliceConfigs[index]);
                }
            }
        }

        public void ShowSliceStarEffect(int sliceIndex)
        {
            if (_sliceViews == null || sliceIndex < 0 || sliceIndex >= _sliceViews.Length)
            {
                return;
            }

            HideAllSliceStarEffects();

            WheelSliceView sliceView = _sliceViews[sliceIndex];

            if (sliceView)
            {
                sliceView.ShowStarEffect();
            }
        }

        public void HideAllSliceStarEffects()
        {
            if (_sliceViews == null)
            {
                return;
            }

            for (int index = 0; index < _sliceViews.Length; index++)
            {
                WheelSliceView sliceView = _sliceViews[index];

                if (sliceView)
                {
                    sliceView.HideStarEffect();
                }
            }
        }

        public void ShowZone(ZoneDisplayData zoneData, bool animateTransition)
        {
            if (_spinText)
            {
                _spinText.text = zoneData.TitleText;
                _spinText.color = zoneData.TextColor;
            }

            if (_statusText)
            {
                _statusText.text = zoneData.StatusText;
                _statusText.color = zoneData.TextColor;
            }

            if (!_backgroundImage)
            {
                return;
            }

            KillTweenIfActive(ref _backgroundColorTween);

            if (animateTransition && _spriteTransitionDuration > ZERO_ALPHA)
            {
                _backgroundColorTween = DOTween.To(
                    () => _backgroundImage.color,
                    value => _backgroundImage.color = value,
                    zoneData.BackgroundColor,
                    _spriteTransitionDuration);
            }
            else
            {
                _backgroundImage.color = zoneData.BackgroundColor;
            }
        }

        public void SetWheelVisuals(Sprite wheelSprite, Sprite pointerSprite, bool animateTransition)
        {
            ApplySpriteTransition(_wheelBackgroundImage, wheelSprite, ref _wheelTransitionTween, animateTransition);
            ApplySpriteTransition(_indicatorImage, pointerSprite, ref _pointerTransitionTween, animateTransition);
        }

        public void SetButtonsInteractable(bool canSpin, bool canLeave)
        {
            if (_spinButton)
            {
                _spinButton.interactable = canSpin;
            }

            if (_leaveButton)
            {
                _leaveButton.gameObject.SetActive(canLeave);
            }

            if (_wheelSpinAnimator)
            {
                _wheelSpinAnimator.SetIdleRotationActive(canSpin);
            }
        }

        private void HandleSpinClicked()
        {
            _hapticService?.Light();
            SpinRequested?.Invoke();
        }

        private void HandleLeaveClicked()
        {
            _hapticService?.Light();
            _audioService?.PlayButtonLeave();
            LeaveRequested?.Invoke();
        }
        
        private void ApplySpriteTransition(Image targetImage, Sprite targetSprite, ref Tween transitionTween, bool animateTransition)
        {
            if (!targetImage)
            {
                return;
            }

            if (!targetSprite || targetImage.sprite == targetSprite)
            {
                return;
            }

            if (!animateTransition || _spriteTransitionDuration <= ZERO_ALPHA)
            {
                targetImage.sprite = targetSprite;
                return;
            }

            KillTweenIfActive(ref transitionTween);

            Color originalColor = targetImage.color;
            float originalAlpha = originalColor.a;
            float halfDuration = _spriteTransitionDuration * HALF_TRANSITION_RATIO;

            Sequence sequence = DOTween.Sequence();
            sequence.Append(CreateAlphaTween(targetImage, originalAlpha, ZERO_ALPHA, halfDuration));
            sequence.AppendCallback(() => targetImage.sprite = targetSprite);
            sequence.Append(CreateAlphaTween(targetImage, ZERO_ALPHA, originalAlpha, halfDuration));
            sequence.OnComplete(() => targetImage.color = originalColor);

            transitionTween = sequence;
        }

        private void KillTransitionTweens()
        {
            KillTweenIfActive(ref _wheelTransitionTween);
            KillTweenIfActive(ref _pointerTransitionTween);
            KillTweenIfActive(ref _backgroundColorTween);
        }

        private static void KillTweenIfActive(ref Tween tween)
        {
            if (tween == null)
            {
                return;
            }

            if (tween.IsActive())
            {
                tween.Kill();
            }

            tween = null;
        }

        private static Tween CreateAlphaTween(Image targetImage, float startAlpha, float endAlpha, float duration)
        {
            return DOVirtual.Float(startAlpha, endAlpha, duration, value =>
            {
                Color fadedColor = targetImage.color;
                fadedColor.a = value;
                targetImage.color = fadedColor;
            });
        }

        private void UnsubscribeFromButtons()
        {
            if (_spinButton)
            {
                _spinButton.onClick.RemoveListener(HandleSpinClicked);
            }

            if (_leaveButton != null)
            {
                _leaveButton.onClick.RemoveListener(HandleLeaveClicked);
            }
        }

        private void OnDisable()
        {
            KillTransitionTweens();
        }

        private void OnDestroy()
        {
            UnsubscribeFromButtons();
            KillTransitionTweens();
        }
    }
}
