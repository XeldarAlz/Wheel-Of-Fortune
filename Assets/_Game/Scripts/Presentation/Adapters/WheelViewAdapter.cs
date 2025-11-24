using UnityEngine;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Zone;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.View.Components;

namespace WheelOfFortune.Presentation.Adapters
{
    [RequireComponent(typeof(WheelView))]
    public sealed class WheelViewAdapter : MonoBehaviour, IWheelView
    {
        private WheelView _view;
        private IWheelGameService _gameService;

        public WheelView View => _view;

        private void Awake()
        {
            _view = GetComponent<WheelView>();
            ServiceLocator.Instance.Register<IWheelView>(this);
        }

        private void OnEnable()
        {
            SubscribeToHudView();
        }

        private void Start()
        {
            ResolveGameService();
        }

        private void SubscribeToHudView()
        {
            if (_view == null)
            {
                return;
            }
            
            _view.SpinRequested += HandleSpinRequested;
            _view.LeaveRequested += HandleLeaveRequested;
        }

        public void RenderSlices(WheelSliceConfig[] sliceConfigs)
        {
            _view?.RenderSlices(sliceConfigs);
        }

        public void ShowZone(ZoneDisplayData zoneData, bool animateTransition)
        {
            _view?.ShowZone(zoneData, animateTransition);
        }

        public void SetWheelVisuals(Sprite wheelSprite, Sprite pointerSprite, bool animateTransition)
        {
            _view?.SetWheelVisuals(wheelSprite, pointerSprite, animateTransition);
        }

        public void SetButtonsInteractable(bool canSpin, bool canLeave)
        {
            _view?.SetButtonsInteractable(canSpin, canLeave);
        }

        public void ShowSliceStarEffect(int sliceIndex)
        {
            _view?.ShowSliceStarEffect(sliceIndex);
        }

        public void HideAllSliceStarEffects()
        {
            _view?.HideAllSliceStarEffects();
        }

        private void HandleSpinRequested()
        {
            ResolveGameService();
            _gameService?.RequestSpin();
        }

        private void HandleLeaveRequested()
        {
            ResolveGameService();
            _gameService?.RequestLeave();
        }

        private void ResolveGameService()
        {
            ServiceResolver.Resolve(ref _gameService);
        }

        private void UnsubscribeFromHudView()
        {
            if (_view == null)
            {
                return;
            }
            
            _view.SpinRequested -= HandleSpinRequested;
            _view.LeaveRequested -= HandleLeaveRequested;
        }
        
        private void OnDisable()
        {
            UnsubscribeFromHudView();
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.Unregister<IWheelView>();
        }
    }
}
