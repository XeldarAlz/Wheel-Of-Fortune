using UnityEngine;

namespace WheelOfFortune.Data.Rewards
{
    public sealed class CollectedRewardItem
    {
        private readonly string _itemId;
        private readonly Sprite _icon;
        private readonly WheelRewardType _rewardType;

        private int _totalAmount;

        public string ItemId => _itemId;
        public Sprite Icon => _icon;
        public WheelRewardType RewardType => _rewardType;
        public bool IsCurrency => _rewardType == WheelRewardType.Currency;
        public int TotalAmount => _totalAmount;

        public CollectedRewardItem(string itemId, Sprite icon, WheelRewardType rewardType, int initialAmount)
        {
            _itemId = itemId;
            _icon = icon;
            _rewardType = rewardType;
            _totalAmount = initialAmount;
        }

        public void AddAmount(int amount)
        {
            _totalAmount += amount;
        }

        public int RemoveAmount(int amount)
        {
            if (amount <= 0)
            {
                return 0;
            }

            int removed = Mathf.Min(_totalAmount, amount);
            _totalAmount -= removed;
            return removed;
        }
    }
}
