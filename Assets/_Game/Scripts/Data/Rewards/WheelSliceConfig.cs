using System;
using UnityEngine;

namespace WheelOfFortune.Data.Rewards
{
    [Serializable]
    public struct WheelSliceConfig
    {
        [SerializeField] private string _sliceId;
        [SerializeField] private Sprite _icon;
        [SerializeField] private WheelRewardType _rewardType;
        [SerializeField] private int _rewardValue;

        public string SliceId => _sliceId;
        public Sprite Icon => _icon;
        public WheelRewardType RewardType => _rewardType;
        public int RewardValue => _rewardValue;
        public bool IsBomb => _rewardType == WheelRewardType.Bomb;

        public WheelSliceConfig(string sliceId, Sprite icon, WheelRewardType rewardType, int rewardValue)
        {
            _sliceId = sliceId;
            _icon = icon;
            _rewardType = rewardType;
            _rewardValue = rewardValue;
        }
    }
}
