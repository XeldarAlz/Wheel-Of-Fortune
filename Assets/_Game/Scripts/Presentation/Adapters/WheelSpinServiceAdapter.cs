using System;
using UnityEngine;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.View.Components;

namespace WheelOfFortune.Presentation.Adapters
{
    [RequireComponent(typeof(WheelSpinAnimator))]
    public sealed class WheelSpinServiceAdapter : MonoBehaviour, IWheelSpinService
    {
        private WheelSpinAnimator _spinAnimator;

        public event Action<int> SpinCompleted;

        public bool IsSpinning => _spinAnimator != null && _spinAnimator.IsSpinning;

        private void Awake()
        {
            _spinAnimator = GetComponent<WheelSpinAnimator>();
            ServiceLocator.Instance.Register<IWheelSpinService>(this);
        }

        private void OnEnable()
        {
            SubscribeToSpinAnimator();
        }

        private void SubscribeToSpinAnimator()
        {
            if (_spinAnimator == null)
            {
                return;
            }

            _spinAnimator.SpinCompleted += HandleSpinCompleted;
        }

        public void SpinToSlice(int targetSliceIndex, int totalSlices)
        {
            _spinAnimator?.SpinToSlice(targetSliceIndex, totalSlices);
        }

        private void HandleSpinCompleted(int sliceIndex)
        {
            SpinCompleted?.Invoke(sliceIndex);
        }

        private void UnsubscribeFromSpinAnimator()
        {
            if (_spinAnimator == null)
            {
                return;
            }

            _spinAnimator.SpinCompleted -= HandleSpinCompleted;
        }

        private void OnDisable()
        {
            UnsubscribeFromSpinAnimator();
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.Unregister<IWheelSpinService>();
        }
    }
}
