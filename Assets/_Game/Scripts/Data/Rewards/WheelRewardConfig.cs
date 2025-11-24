using UnityEngine;

namespace WheelOfFortune.Data.Rewards
{
    [CreateAssetMenu(fileName = "WheelRewardConfig", menuName = "WheelOfFortune/Wheel Reward Config")]
    public sealed class WheelRewardConfig : ScriptableObject
    {
        private const int MINIMUM_SLICES = 8;

        [SerializeField] private SliceRewardData[] _bronzeSlices;
        [SerializeField] private SliceRewardData[] _silverSlices;
        [SerializeField] private SliceRewardData[] _superSlices;
        [SerializeField] private RewardItemDefinition _bombItem;

        private WheelSliceConfig[] _bronzeSlicesCache;
        private WheelSliceConfig[] _silverSlicesCache;
        private WheelSliceConfig[] _superSlicesCache;
        private WheelSliceConfig _bombSliceCache;
        private bool _cacheInitialized;

        public WheelSliceConfig[] BronzeSlices
        {
            get
            {
                InitializeCacheIfNeeded();
                return _bronzeSlicesCache;
            }
        }

        public WheelSliceConfig[] SilverSlices
        {
            get
            {
                InitializeCacheIfNeeded();
                return _silverSlicesCache;
            }
        }

        public WheelSliceConfig[] SuperSlices
        {
            get
            {
                InitializeCacheIfNeeded();
                return _superSlicesCache;
            }
        }

        public WheelSliceConfig BombSlice
        {
            get
            {
                InitializeCacheIfNeeded();
                return _bombSliceCache;
            }
        }

        public bool HasValidBaseline()
        {
            return _bronzeSlices != null && _bronzeSlices.Length >= MINIMUM_SLICES;
        }

        private void InitializeCacheIfNeeded()
        {
            if (_cacheInitialized)
            {
                return;
            }

            _bronzeSlicesCache = ConvertToSliceConfigs(_bronzeSlices);
            _silverSlicesCache = ConvertToSliceConfigs(_silverSlices);
            _superSlicesCache = ConvertToSliceConfigs(_superSlices);
            
            if (_bombItem != null)
            {
                _bombSliceCache = new WheelSliceConfig(_bombItem.ItemId, _bombItem.Icon, _bombItem.RewardType, 0);
            }

            _cacheInitialized = true;
        }

        private WheelSliceConfig[] ConvertToSliceConfigs(SliceRewardData[] sliceData)
        {
            if (sliceData == null)
            {
                return null;
            }

            WheelSliceConfig[] configs = new WheelSliceConfig[sliceData.Length];

            for (int index = 0; index < sliceData.Length; index++)
            {
                configs[index] = sliceData[index].ToSliceConfig();
            }

            return configs;
        }
    }
}
