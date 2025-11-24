using System;
using UnityEngine;

namespace WheelOfFortune.Data.Rewards
{
    [Serializable]
    public struct SliceRewardData
    {
        [SerializeField] private RewardItemDefinition _rewardItem;
        [SerializeField] private int _amount;

        public SliceRewardData(RewardItemDefinition rewardItem, int amount)
        {
            _rewardItem = rewardItem;
            _amount = amount;
        }

        public WheelSliceConfig ToSliceConfig()
        {
            if (_rewardItem == null)
            {
                return default;
            }

            return new WheelSliceConfig(_rewardItem.ItemId, _rewardItem.Icon, _rewardItem.RewardType, _amount);
        }
    }
}
