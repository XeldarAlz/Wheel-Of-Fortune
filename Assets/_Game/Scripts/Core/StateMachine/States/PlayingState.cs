using UnityEngine;
using WheelOfFortune.Core.Models;
using WheelOfFortune.Core.Strategies;
using WheelOfFortune.Data.Profiles;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Zone;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;

namespace WheelOfFortune.Core.StateMachine.States
{
    public sealed class PlayingState : IWheelGameState
    {
        private readonly WheelGameModel _model;
        private readonly WheelProgressionProfile _progressionProfile;
        private readonly ZoneStrategyFactory _zoneStrategyFactory;
        
        private IWheelSpinService _spinService;
        private ISpinRandomizer _spinRandomizer;
        private IWheelView _view;
        private IWheelGameService _gameService;
        private IAudioService _audioService;
        private IScreenService _screenService;
        
        private ZoneType _currentZoneType;
        private bool _hasInitializedZoneType;

        public PlayingState(WheelGameModel model, WheelProgressionProfile progressionProfile)
        {
            _model = model;
            _progressionProfile = progressionProfile;
            _zoneStrategyFactory = new ZoneStrategyFactory(progressionProfile);
        }

        public void Enter()
        {
            PrepareZone();
        }

        public void Exit()
        {
        }

        public void HandleSpinRequested()
        {
            ResolveSpinService();
            ResolveWheelView();
            ResolveGameService();

            if (_spinService == null || _spinService.IsSpinning || _model.PendingBomb)
            {
                return;
            }

            _view?.SetButtonsInteractable(false, false);

            int sliceIndex = 0;
            
            if (_gameService != null)
            {
                SpinRequest activeRequest = _gameService.GetActiveSpinRequest();
                
                if (activeRequest != null)
                {
                    sliceIndex = activeRequest.TargetSliceIndex;
                }
                else
                {
                    sliceIndex = ResolveSpinOutcome();
                }
            }
            else
            {
                sliceIndex = ResolveSpinOutcome();
            }

            ResolveAudioService();
            _audioService?.PlayWheelRotate();
            _spinService.SpinToSlice(sliceIndex, _model.SliceCount);
        }

        public void HandleLeaveRequested()
        {
            ResolveSpinService();

            if (_spinService != null && _spinService.IsSpinning)
            {
                return;
            }

            _model.ResetGame();
            
            ResolveGameService();
            _gameService?.ClearCollectedRewards();
            
            PrepareZone();
        }

        public void HandleContinueRequested()
        {
        }

        public void HandleGiveUpRequested()
        {
        }

        private void PrepareZone()
        {
            if (_progressionProfile == null || !_progressionProfile.HasValidBaseline())
            {
                return;
            }

            ResolveWheelView();

            _model.SetZoneIntervals(_progressionProfile.SafeZoneInterval, _progressionProfile.SuperZoneInterval);

            IZoneTypeStrategy zoneStrategy = _zoneStrategyFactory.GetStrategy(_model);
            WheelSliceConfig[] baseSlices = zoneStrategy.GetSlices();
            _model.SetSlices(baseSlices, _progressionProfile.BombSlice);

            ZoneDisplayData zoneData = new(_model.CurrentZoneIndex, zoneStrategy.GetTitleText(), 
                zoneStrategy.GetStatusText(), zoneStrategy.GetTextColor(), zoneStrategy.GetBackgroundColor());
            
            ZoneType resolvedZoneType = ResolveZoneType();
            bool zoneChanged = _hasInitializedZoneType && resolvedZoneType != _currentZoneType;
            _currentZoneType = resolvedZoneType;
            _hasInitializedZoneType = true;

            if (zoneChanged)
            {
                PlayZoneChangeAudio(resolvedZoneType);
            }

            if (_view != null)
            {
                _view.RenderSlices(_model.WorkingSlices);
                _view.ShowZone(zoneData, zoneChanged);
                _view.SetWheelVisuals(zoneStrategy.GetWheelSprite(), zoneStrategy.GetIndicatorSprite(), zoneChanged);
                
                UpdateButtonStates();
            }

            ResolveScreenService();
            _screenService?.ShowWheelScreen();
        }

        private void UpdateButtonStates()
        {
            ResolveWheelView();
            ResolveSpinService();

            if (_view == null)
            {
                return;
            }

            bool isSpinning = _spinService != null && _spinService.IsSpinning;
            bool canSpin = !isSpinning && !_model.PendingBomb;
            bool hasPendingRewards = _model.PendingReward > 0;
            bool canLeave = canSpin && hasPendingRewards;

            _view.SetButtonsInteractable(canSpin, canLeave);
        }

        private void ResolveScreenService()
        {
            ServiceResolver.Resolve(ref _screenService);
        }

        private ZoneType ResolveZoneType()
        {
            if (_model.IsSuperZone)
            {
                return ZoneType.Super;
            }

            if (_model.IsSafeZone)
            {
                return ZoneType.Silver;
            }

            return ZoneType.Bronze;
        }

        private int ResolveSpinOutcome()
        {
            ResolveSpinRandomizer();

            if (_spinRandomizer == null)
            {
                return Random.Range(0, _model.SliceCount);
            }

            WheelSliceConfig[] workingSlices = _model.WorkingSlices;
            
            int resolvedIndex = _spinRandomizer.ResolveSliceIndex(workingSlices, _model.CurrentZoneIndex);
            int clampedIndex = Mathf.Clamp(resolvedIndex, 0, _model.SliceCount - 1);

            return clampedIndex;
        }

        private void ResolveSpinRandomizer()
        {
            ServiceResolver.Resolve(ref _spinRandomizer);
        }

        private void ResolveSpinService()
        {
            ServiceResolver.Resolve(ref _spinService);
        }

        private void ResolveWheelView()
        {
            ServiceResolver.Resolve(ref _view);
        }

        private void ResolveGameService()
        {
            ServiceResolver.Resolve(ref _gameService);
        }

        private void ResolveAudioService()
        {
            ServiceResolver.Resolve(ref _audioService);
        }

        private void PlayZoneChangeAudio(ZoneType zoneType)
        {
            ResolveAudioService();

            if (_audioService == null)
            {
                return;
            }

            switch (zoneType)
            {
                case ZoneType.Silver:
                    _audioService.PlaySilverZoneEnter();
                    break;
                case ZoneType.Super:
                    _audioService.PlaySuperZoneEnter();
                    break;
                case ZoneType.Bronze:
                    break;
            }
        }
    }
}
