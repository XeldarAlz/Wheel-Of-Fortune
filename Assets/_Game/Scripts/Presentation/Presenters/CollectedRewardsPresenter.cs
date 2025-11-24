using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Infrastructure.Pooling;
using WheelOfFortune.View.Components;

namespace WheelOfFortune.Presentation.Presenters
{
    public sealed class CollectedRewardsPresenter
    {
        private readonly Dictionary<string, CollectedRewardItem> _collectedItems;
        private readonly Dictionary<string, CollectedRewardItemView> _activeViews;
        private readonly RewardItemPool _itemPool;
        private readonly GameObject _labelObject;

        public CollectedRewardsPresenter(CollectedRewardItemView prefab, Transform parent, GameObject labelObject)
        {
            _collectedItems = new Dictionary<string, CollectedRewardItem>();
            _activeViews = new Dictionary<string, CollectedRewardItemView>();
            _itemPool = new RewardItemPool(prefab, parent);
            _labelObject = labelObject;
            
            UpdateLabelVisibility();
        }

        public void AddReward(WheelSliceConfig rewardConfig)
        {
            if (rewardConfig.IsBomb)
            {
                return;
            }

            string itemId = rewardConfig.SliceId;

            if (_collectedItems.TryGetValue(itemId, out CollectedRewardItem existingItem))
            {
                existingItem.AddAmount(rewardConfig.RewardValue);

                if (_activeViews.TryGetValue(itemId, out CollectedRewardItemView view))
                {
                    view.UpdateAmount(existingItem.TotalAmount);
                }
            }
            else
            {
                CollectedRewardItem newItem = new(rewardConfig.SliceId, rewardConfig.Icon, rewardConfig.RewardType,
                    rewardConfig.RewardValue);

                _collectedItems[itemId] = newItem;

                CollectedRewardItemView view = _itemPool.Get();
                view.Initialize(newItem.Icon, newItem.TotalAmount);
                _activeViews[itemId] = view;
            }
            
            UpdateLabelVisibility();
        }

        public void ClearAll()
        {
            _collectedItems.Clear();

            foreach (KeyValuePair<string, CollectedRewardItemView> pair in _activeViews)
            {
                _itemPool.Return(pair.Value);
            }

            _activeViews.Clear();
            
            UpdateLabelVisibility();
        }

        public int GetTotalCurrency()
        {
            int total = 0;

            foreach (CollectedRewardItem item in _collectedItems.Values)
            {
                if (item.IsCurrency)
                {
                    total += item.TotalAmount;
                }
            }

            return total;
        }

        public void SpendCurrency(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            int remaining = amount;
            List<string> keys = new(_collectedItems.Keys);

            for (int index = 0; index < keys.Count; index++)
            {
                string key = keys[index];
                if (!_collectedItems.TryGetValue(key, out CollectedRewardItem item))
                {
                    continue;
                }

                if (!item.IsCurrency)
                {
                    continue;
                }

                int removed = item.RemoveAmount(remaining);
                remaining -= removed;

                if (_activeViews.TryGetValue(key, out CollectedRewardItemView view))
                {
                    if (item.TotalAmount > 0)
                    {
                        view.UpdateAmount(item.TotalAmount);
                    }
                    else
                    {
                        _itemPool.Return(view);
                        _activeViews.Remove(key);
                    }
                }

                if (item.TotalAmount <= 0)
                {
                    _collectedItems.Remove(key);
                }

                if (remaining <= 0)
                {
                    break;
                }
            }

            UpdateLabelVisibility();
        }
        
        private void UpdateLabelVisibility()
        {
            if (!_labelObject)
            {
                return;
            }
            
            _labelObject.SetActive(_activeViews.Count > 0);
        }
    }
}
