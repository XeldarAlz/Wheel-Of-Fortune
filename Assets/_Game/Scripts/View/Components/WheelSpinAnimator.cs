using System;
using UnityEngine;
using DG.Tweening;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;

namespace WheelOfFortune.View.Components
{
    public sealed class WheelSpinAnimator : MonoBehaviour
    {
        private const float FULL_CIRCLE_DEGREES = 360.0f;
        private const float HAPTIC_ANGLE_THRESHOLD = 15.0f;
        private const float IDLE_ROTATION_DURATION_SECONDS = 30.0f;
        
        private static readonly Vector3 IdleRotationVector = new(0.0f, 0.0f, 360.0f);

        [SerializeField] private RectTransform _wheelTransform;
        [SerializeField] private AnimationCurve _spinCurve;
        [SerializeField] private float _spinDuration = 4.0f;
        [SerializeField] private int _minimumExtraRotations = 2;
        [SerializeField] private int _maximumExtraRotations = 4;

        private Tween _spinTween;
        private Tween _wheelIdleRotationTween;
        private bool _isSpinning;
        private Vector3 _cachedRotation;
        private float _lastHapticAngle;
        private IHapticFeedbackService _hapticService;
        private int _targetSliceIndex;

        public event Action<int> SpinCompleted;

        public bool IsSpinning => _isSpinning;

        private void Start()
        {
            ResolveHapticService();
        }

        public void SpinToSlice(int targetSliceIndex, int totalSlices)
        {
            if (_isSpinning)
            {
                return;
            }

            if (totalSlices <= 0)
            {
                return;
            }

            if (!_wheelTransform)
            {
                return;
            }

            float sliceAngle = FULL_CIRCLE_DEGREES / totalSlices;
            float currentAngle = _wheelTransform.eulerAngles.z;
            int extraRotations = UnityEngine.Random.Range(_minimumExtraRotations, _maximumExtraRotations + 1);

            float targetCenterAngle = targetSliceIndex * sliceAngle;
            
            float angleToTarget = targetCenterAngle - (currentAngle % FULL_CIRCLE_DEGREES);
            
            if (angleToTarget <= 0.0f)
            {
                angleToTarget += FULL_CIRCLE_DEGREES;
            }

            float totalRotation = currentAngle + angleToTarget + (extraRotations * FULL_CIRCLE_DEGREES);

            _targetSliceIndex = targetSliceIndex;

            StopSpinTween();
            _isSpinning = true;

            ResolveHapticService();
            _hapticService?.Medium();
            _lastHapticAngle = currentAngle;

            _spinTween = DOTween.To(() => currentAngle, SetWheelRotation, totalRotation, _spinDuration)
                .SetEase(_spinCurve)
                .OnUpdate(OnSpinUpdate)
                .OnComplete(() => OnSpinComplete());
        }

        public void SetIdleRotationActive(bool isActive)
        {
            if (isActive)
            {
                StartIdleRotation();
            }
            else
            {
                StopIdleRotation();
            }
        }

        private void SetWheelRotation(float angleValue)
        {
            _cachedRotation.z = angleValue;
            _wheelTransform.eulerAngles = _cachedRotation;
        }

        private void OnSpinUpdate()
        {
            float currentAngle = _wheelTransform.eulerAngles.z;
            float angleDifference = Mathf.Abs(currentAngle - _lastHapticAngle);

            if (!(angleDifference >= HAPTIC_ANGLE_THRESHOLD))
            {
                return;
            }

            ResolveHapticService();
            _hapticService?.Light();
            _lastHapticAngle = currentAngle;
        }

        private void OnSpinComplete()
        {
            ResolveHapticService();
            _hapticService?.Heavy();
            int resultSliceIndex = _targetSliceIndex;
            _isSpinning = false;
            SpinCompleted?.Invoke(resultSliceIndex);
        }

        private void ResolveHapticService()
        {
            ServiceResolver.Resolve(ref _hapticService);
        }

        private void StopSpinTween()
        {
            if (_spinTween == null)
            {
                return;
            }

            if (_spinTween.IsActive())
            {
                _spinTween.Kill();
            }

            _spinTween = null;
            _isSpinning = false;
        }

        private void StartIdleRotation()
        {
            if (!_wheelTransform)
            {
                return;
            }

            if (_wheelIdleRotationTween != null && _wheelIdleRotationTween.IsActive())
            {
                return;
            }

            _wheelIdleRotationTween = _wheelTransform
                .DORotate(IdleRotationVector, IDLE_ROTATION_DURATION_SECONDS, RotateMode.FastBeyond360)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart);
        }

        private void StopIdleRotation()
        {
            if (_wheelIdleRotationTween == null)
            {
                return;
            }

            if (_wheelIdleRotationTween.IsActive())
            {
                _wheelIdleRotationTween.Kill();
            }

            _wheelIdleRotationTween = null;
        }
        
        private void OnDisable()
        {
            StopSpinTween();
            StopIdleRotation();
            _cachedRotation = Vector3.zero;
        }

        private void OnDestroy()
        {
            StopIdleRotation();
        }
    }
}
