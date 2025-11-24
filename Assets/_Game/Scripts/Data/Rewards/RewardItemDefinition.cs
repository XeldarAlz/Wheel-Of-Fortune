using UnityEngine;

namespace WheelOfFortune.Data.Rewards
{
    [CreateAssetMenu(fileName = "RewardItem", menuName = "WheelOfFortune/Reward Item Definition")]
    public sealed class RewardItemDefinition : ScriptableObject
    {
        [SerializeField] private string _itemId;
        [SerializeField] private Sprite _icon;
        [SerializeField] private WheelRewardType _rewardType;

        public string ItemId => _itemId;
        public Sprite Icon => _icon;
        public WheelRewardType RewardType => _rewardType;
        public bool IsBomb => _rewardType == WheelRewardType.Bomb;
    }
}
