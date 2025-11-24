using System.Collections.Generic;
using UnityEngine;
using WheelOfFortune.View.Components;

namespace WheelOfFortune.Infrastructure.Pooling
{
    public sealed class RewardItemPool
    {
        private readonly CollectedRewardItemView _prefab;
        private readonly Transform _parent;
        private readonly Queue<CollectedRewardItemView> _availableItems;
        private readonly HashSet<CollectedRewardItemView> _activeItems;
        private readonly Transform _poolRoot;
        private readonly int _prewarmCount;

        public RewardItemPool(CollectedRewardItemView prefab, Transform parent, int prewarmCount = 20)
        {
            _prefab = prefab;
            _parent = parent;
            _availableItems = new Queue<CollectedRewardItemView>();
            _activeItems = new HashSet<CollectedRewardItemView>();
            _poolRoot = CreatePoolRoot(parent, prefab);
            _prewarmCount = Mathf.Max(0, prewarmCount);

            Prewarm();
        }

        public CollectedRewardItemView Get()
        {
            CollectedRewardItemView item;

            if (_availableItems.Count > 0)
            {
                item = _availableItems.Dequeue();
                item.transform.SetParent(_parent, false);
                item.gameObject.SetActive(true);
            }
            else
            {
                item = Object.Instantiate(_prefab, _parent);
            }

            _activeItems.Add(item);
            return item;
        }

        public void Return(CollectedRewardItemView item)
        {
            if (!item)
            {
                return;
            }

            if (!_activeItems.Remove(item))
            {
                return;
            }

            item.ResetView();
            if (_poolRoot)
            {
                item.transform.SetParent(_poolRoot, false);
            }
            else if (_parent)
            {
                item.transform.SetParent(_parent, false);
            }
            item.gameObject.SetActive(false);
            _availableItems.Enqueue(item);
        }

        private static Transform CreatePoolRoot(Transform parent, CollectedRewardItemView prefab)
        {
            if (parent == null)
            {
                return null;
            }

            string poolName = prefab != null ? prefab.name : "RewardItem";
            GameObject poolRootObject = new($"{poolName}_Pool");
            Transform poolRoot = poolRootObject.transform;

            Transform poolParent = parent.parent != null ? parent.parent : parent;
            poolRoot.SetParent(poolParent, false);
            poolRoot.gameObject.SetActive(false);

            return poolRoot;
        }

        private void Prewarm()
        {
            if (_prefab == null)
            {
                return;
            }

            for (int index = 0; index < _prewarmCount; index++)
            {
                CollectedRewardItemView item = Object.Instantiate(_prefab, _poolRoot != null ? _poolRoot : _parent);
                item.gameObject.SetActive(false);
                _availableItems.Enqueue(item);
            }
        }
    }
}
