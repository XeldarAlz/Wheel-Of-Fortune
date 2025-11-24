using UnityEngine;
using WheelOfFortune.Data.Rewards;

namespace WheelOfFortune.Core.Models
{
    public sealed class WheelGameModel
    {
        private const int SLICE_COUNT = 8;
        private const int BASE_CONTINUE_COST = 200;
        private const int INITIAL_ZONE_INDEX = 1;
        private const int INITIAL_CONTINUES_USED = 0;
        private const int MINIMUM_CONTINUE_MULTIPLIER = 1;

        private readonly WheelSliceConfig[] _workingSlices;
        private readonly WheelRewardBank _rewardBank;

        private int _currentZoneIndex;
        private int _continuesUsed;
        private int _safeZoneInterval;
        private int _superZoneInterval;
        private bool _pendingBomb;
        
        public int SliceCount => SLICE_COUNT;
        public int CurrentZoneIndex => _currentZoneIndex;
        public int PendingReward => _rewardBank.PendingReward;
        public bool PendingBomb => _pendingBomb;
        public bool IsSafeZone => _currentZoneIndex % _superZoneInterval != 0 && _currentZoneIndex % _safeZoneInterval == 0;
        public bool IsSuperZone => _currentZoneIndex % _superZoneInterval == 0;
        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;
        public WheelSliceConfig[] WorkingSlices => _workingSlices;

        public WheelGameModel(int safeZoneInterval = 5, int superZoneInterval = 30)
        {
            _workingSlices = new WheelSliceConfig[SLICE_COUNT];
            _rewardBank = new WheelRewardBank();
            _currentZoneIndex = INITIAL_ZONE_INDEX;
            _continuesUsed = INITIAL_CONTINUES_USED;
            _pendingBomb = false;
            _safeZoneInterval = safeZoneInterval;
            _superZoneInterval = superZoneInterval;
        }

        public void SetZoneIntervals(int safeZoneInterval, int superZoneInterval)
        {
            _safeZoneInterval = safeZoneInterval;
            _superZoneInterval = superZoneInterval;
        }

        public void ResetGame()
        {
            _currentZoneIndex = INITIAL_ZONE_INDEX;
            _continuesUsed = INITIAL_CONTINUES_USED;
            _pendingBomb = false;
            _rewardBank.ClearPending();
        }

        public void SetSlices(WheelSliceConfig[] slices, WheelSliceConfig bombSlice)
        {
            if (slices == null || slices.Length == 0)
            {
                return;
            }

            int sourceLength = slices.Length;
            int fallbackIndex = sourceLength - 1;

            for (int index = 0; index < SLICE_COUNT; index++)
            {
                if (index >= sourceLength)
                {
                    _workingSlices[index] = slices[fallbackIndex];
                    continue;
                }

                WheelSliceConfig sourceSlice = slices[index];
                _workingSlices[index] = sourceSlice.IsBomb ? bombSlice : sourceSlice;
            }
        }

        public void AddReward(int value)
        {
            _rewardBank.AddReward(value);
        }

        public void ClearPendingRewards()
        {
            _rewardBank.ClearPending();
        }

        public bool TrySpendCurrency(int amount)
        {
            return _rewardBank.TrySpend(amount);
        }

        public void SetPendingBomb(bool value)
        {
            _pendingBomb = value;
        }

        public int GetContinueCost()
        {
            int multiplier = Mathf.Max(MINIMUM_CONTINUE_MULTIPLIER, _continuesUsed + 1);
            return BASE_CONTINUE_COST * multiplier;
        }

        public void AdvanceZone()
        {
            _currentZoneIndex++;
        }

        public void SetZoneIndex(int zoneIndex)
        {
            _currentZoneIndex = Mathf.Max(INITIAL_ZONE_INDEX, zoneIndex);
        }
    }
}
