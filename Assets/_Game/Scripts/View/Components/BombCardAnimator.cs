using UnityEngine;
using DG.Tweening;

namespace WheelOfFortune.View.Components
{
    public sealed class BombCardAnimator : MonoBehaviour
    {
        private const float DEFAULT_ROTATION_ANGLE = 10.0f;
        private const float DEFAULT_ROTATION_DURATION = 0.5f;

        [SerializeField] private Transform _targetTransform;
        [SerializeField] private AnimationCurve _rotationCurve;
        [SerializeField] private float _rotationAngle = DEFAULT_ROTATION_ANGLE;
        [SerializeField] private float _rotationDuration = DEFAULT_ROTATION_DURATION;

        private Tweener _rotationTween;
        private bool _isAnimating;

        public bool IsAnimating => _isAnimating;

        private void OnEnable()
        {
            StartRotation();
        }

        public void StartRotation()
        {
            if (_isAnimating)
            {
                return;
            }

            if (_targetTransform == null)
            {
                return;
            }

            StopRotationTween();

            _targetTransform.localRotation = Quaternion.Euler(0.0f, 0.0f, -_rotationAngle);

            _rotationTween = _targetTransform
                .DOLocalRotate(new Vector3(0.0f, 0.0f, _rotationAngle), _rotationDuration, RotateMode.Fast)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(_rotationCurve);

            _isAnimating = true;
        }

        public void StopRotation()
        {
            StopRotationTween();
            ResetRotation();
        }

        private void StopRotationTween()
        {
            if (_rotationTween == null)
            {
                return;
            }

            if (_rotationTween.IsActive())
            {
                _rotationTween.Kill();
            }

            _rotationTween = null;
            _isAnimating = false;
        }

        private void ResetRotation()
        {
            if (_targetTransform == null)
            {
                return;
            }

            _targetTransform.localRotation = Quaternion.identity;
        }

        private void OnDisable()
        {
            StopRotation();
        }
    }
}
