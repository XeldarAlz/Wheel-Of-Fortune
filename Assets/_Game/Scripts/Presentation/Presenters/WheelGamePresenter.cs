using DG.Tweening;
using UnityEngine;
using WheelOfFortune.Core.Models;
using WheelOfFortune.Core.Services;
using WheelOfFortune.Core.StateMachine;
using WheelOfFortune.Core.StateMachine.States;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Profiles;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Presentation.Presenters
{
    public sealed class WheelGamePresenter : IWheelGameService
    {
        private const float MINIMUM_DELAY_SECONDS = 0.0f;

        private readonly WheelGameModel _model;
        private readonly WheelGameStateMachine _stateMachine;
        private readonly PlayingState _playingState;
        private readonly BombState _bombState;
        private readonly WheelProgressionProfile _progressionProfile;
        private readonly CollectedRewardsPresenter _collectedRewardsPresenter;
        private readonly IWheelSpinService _spinService;
        private readonly IWheelView _view;
        private readonly IHapticFeedbackService _hapticService;
        private readonly ISpinRandomizer _spinRandomizer;
        private readonly IAudioService _audioService;
        private readonly RevivalService _revivalService;

        private Tween _postSpinDelayTween;
        private SpinRequest _pendingSpinRequest;
        private SpinRequest _activeSpinRequest;

        public WheelGamePresenter(WheelGameModel model, WheelGameStateMachine stateMachine, PlayingState playingState,
            BombState bombState, WheelProgressionProfile progressionProfile,
            CollectedRewardsPresenter collectedRewardsPresenter, IWheelSpinService spinService, IWheelView view,
            IHapticFeedbackService hapticService, ISpinRandomizer spinRandomizer, IAudioService audioService,
            RevivalService revivalService)
        {
            _model = model;
            _stateMachine = stateMachine;
            _playingState = playingState;
            _bombState = bombState;
            _progressionProfile = progressionProfile;
            _collectedRewardsPresenter = collectedRewardsPresenter;
            _spinService = spinService;
            _view = view;
            _hapticService = hapticService;
            _spinRandomizer = spinRandomizer;
            _audioService = audioService;
            _revivalService = revivalService;

            SubscribeToSpinService();
        }

        private void SubscribeToSpinService()
        {
            if (_spinService == null)
            {
                return;
            }

            _spinService.SpinCompleted -= HandleSpinCompleted;
            _spinService.SpinCompleted += HandleSpinCompleted;
        }

        public void StartGame()
        {
            _audioService?.PlayBackgroundLoop();
            _stateMachine.ChangeState(_playingState);
        }

        public void RequestSpin()
        {
            if (!CanSpin())
            {
                return;
            }

            if (_pendingSpinRequest != null)
            {
                _activeSpinRequest = _pendingSpinRequest;
                _pendingSpinRequest = null;
            }
            else
            {
                _activeSpinRequest = GenerateRandomSpinRequest();
            }

            _stateMachine.HandleSpinRequested();
        }

        public void RequestLeave()
        {
            _stateMachine.HandleLeaveRequested();
        }

        public void RequestContinue()
        {
            _stateMachine.HandleContinueRequested();
        }

        public void RequestGiveUp()
        {
            _stateMachine.HandleGiveUpRequested();
        }

        public void ForceNextSlice(int sliceIndex)
        {
            if (_model == null)
            {
                return;
            }

            int clampedIndex = Mathf.Clamp(sliceIndex, 0, _model.SliceCount - 1);
            _pendingSpinRequest = new SpinRequest(clampedIndex);
        }

        public void ClearForcedSlice()
        {
            _pendingSpinRequest = null;
            _activeSpinRequest = null;
        }

        public int GetPendingCurrency()
        {
            if (_collectedRewardsPresenter != null)
            {
                int total = _collectedRewardsPresenter.GetTotalCurrency();
                return total;
            }

            if (_model == null)
            {
                return 0;
            }

            int pendingReward = _model.PendingReward;
            return pendingReward;
        }

        public void SpendCurrency(int amount)
        {
            _collectedRewardsPresenter?.SpendCurrency(amount);
        }

        public bool CanSpin()
        {
            return _spinService != null && !_spinService.IsSpinning && !_model.PendingBomb;
        }

        public bool CanLeave()
        {
            return CanSpin();
        }

        public bool CanContinue()
        {
            return _model.PendingBomb && _model.PendingReward >= _model.GetContinueCost();
        }

        public void ClearCollectedRewards()
        {
            _collectedRewardsPresenter?.ClearAll();
        }

        public void ResetToInitialState()
        {
            _model.ResetGame();
            _model.ClearPendingRewards();
            _collectedRewardsPresenter?.ClearAll();
            _view?.HideAllSliceStarEffects();
            _stateMachine.ChangeState(_playingState);
        }

        public void SetZoneToBronze()
        {
            if (_model.IsSafeZone || _model.IsSuperZone)
            {
                _model.SetZoneIndex(1);
                RefreshZoneDisplay();
            }
        }

        public void SetZoneToSilver()
        {
            if (_model.IsSafeZone && !_model.IsSuperZone)
            {
                return;
            }

            int safeZoneIndex = _model.SafeZoneInterval;
            _model.SetZoneIndex(safeZoneIndex);
            RefreshZoneDisplay();
        }

        public void SetZoneToGold()
        {
            if (_model.IsSuperZone)
            {
                return;
            }

            int superZoneIndex = _model.SuperZoneInterval;
            _model.SetZoneIndex(superZoneIndex);
            RefreshZoneDisplay();
        }

        public WheelSliceConfig[] GetWorkingSlices()
        {
            return _model?.WorkingSlices;
        }

        public SpinRequest GetActiveSpinRequest()
        {
            return _activeSpinRequest;
        }

        private void RefreshZoneDisplay()
        {
            _stateMachine.ChangeState(_playingState);
        }

        private void HandleSpinCompleted(int sliceIndex)
        {
            WheelSliceConfig result = _model.WorkingSlices[sliceIndex];
            _audioService?.StopWheelRotate();

            _activeSpinRequest = null;

            if (result.IsBomb)
            {
                HandleBombResult();
                return;
            }

            HandleRewardResult(result, sliceIndex);
        }

        private void HandleBombResult()
        {
            _hapticService?.Error();
            _audioService?.PlayBombHit();
            _audioService?.StopWheelRotate();
            _view?.HideAllSliceStarEffects();
            _stateMachine.ChangeState(_bombState);
        }

        private void HandleRewardResult(WheelSliceConfig result, int sliceIndex)
        {
            _hapticService?.Success();
            _audioService?.PlayWheelReward();

            _model.AddReward(result.RewardValue);
            _collectedRewardsPresenter?.AddReward(result);

            _view?.ShowSliceStarEffect(sliceIndex);

            _model.AdvanceZone();

            KillPostSpinTween();

            float delay = _progressionProfile != null 
                ? Mathf.Max(MINIMUM_DELAY_SECONDS, _progressionProfile.PostSpinDelaySeconds)
                : MINIMUM_DELAY_SECONDS;

            if (delay <= MINIMUM_DELAY_SECONDS)
            {
                _view?.HideAllSliceStarEffects();
                _stateMachine.ChangeState(_playingState);
                return;
            }

            _postSpinDelayTween = DOVirtual.DelayedCall(delay, () =>
            {
                _view?.HideAllSliceStarEffects();
                _stateMachine.ChangeState(_playingState);
            });
        }

        private void KillPostSpinTween()
        {
            if (_postSpinDelayTween == null)
            {
                return;
            }

            if (_postSpinDelayTween.IsActive())
            {
                _postSpinDelayTween.Kill();
            }

            _postSpinDelayTween = null;
        }

        private SpinRequest GenerateRandomSpinRequest()
        {
            if (_model == null || _spinRandomizer == null)
            {
                int randomSlice = Random.Range(0, _model?.SliceCount ?? 8);
                return new SpinRequest(randomSlice);
            }

            WheelSliceConfig[] workingSlices = _model.WorkingSlices;
            int resolvedIndex = _spinRandomizer.ResolveSliceIndex(workingSlices, _model.CurrentZoneIndex);
            int clampedIndex = Mathf.Clamp(resolvedIndex, 0, _model.SliceCount - 1);

            return new SpinRequest(clampedIndex);
        }

        private void UnsubscribeFromSpinService()
        {
            if (_spinService == null)
            {
                return;
            }

            _spinService.SpinCompleted -= HandleSpinCompleted;
        }
        
        public void Dispose()
        {
            UnsubscribeFromSpinService();
            KillPostSpinTween();
            _audioService?.StopBackgroundLoop();
        }
    }
}
