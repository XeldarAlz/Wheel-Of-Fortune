using UnityEngine;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Zone;

namespace WheelOfFortune.Data.Profiles
{
    [CreateAssetMenu(fileName = "WheelProgressionProfile", menuName = "WheelOfFortune/Wheel Progression Profile")]
    public sealed class WheelProgressionProfile : ScriptableObject
    {
        private const float DEFAULT_POST_SPIN_DELAY_SECONDS = 1.0f;
        private const float MINIMUM_DELAY_SECONDS = 0.0f;
        private const float MAXIMUM_DELAY_SECONDS = 10.0f;

        private static readonly Color DefaultSilverColor = new(0.48f, 0.48f, 0.48f, 1.0f);
        private static readonly Color DefaultSuperColor = new(0.93f, 0.64f, 0.05f, 1.0f);

        [Header("Configuration References")]
        [SerializeField] private ZoneProgressionConfig _progressionConfig;
        [SerializeField] private ZoneUIConfig _uiConfig;
        [SerializeField] private WheelRewardConfig _rewardConfig;
        [Header("Timing")]
        [SerializeField]
        [Range(MINIMUM_DELAY_SECONDS, MAXIMUM_DELAY_SECONDS)]
        private float _postSpinDelaySeconds = DEFAULT_POST_SPIN_DELAY_SECONDS;

        public int SafeZoneInterval => _progressionConfig != null ? _progressionConfig.SafeZoneInterval : 5;
        public int SuperZoneInterval => _progressionConfig != null ? _progressionConfig.SuperZoneInterval : 30;
        public float PostSpinDelaySeconds => _postSpinDelaySeconds;
        public ZoneVisualConfig BronzeVisualConfig => _uiConfig != null ? _uiConfig.BronzeVisualConfig : null;
        public ZoneVisualConfig SilverVisualConfig => _uiConfig != null ? _uiConfig.SilverVisualConfig : null;
        public ZoneVisualConfig SuperVisualConfig => _uiConfig != null ? _uiConfig.SuperVisualConfig : null;
        public WheelSliceConfig[] BronzeSlices => _rewardConfig != null ? _rewardConfig.BronzeSlices : null;
        public WheelSliceConfig[] SilverSlices => _rewardConfig != null ? _rewardConfig.SilverSlices : null;
        public WheelSliceConfig[] SuperSlices => _rewardConfig != null ? _rewardConfig.SuperSlices : null;
        public WheelSliceConfig BombSlice => _rewardConfig != null ? _rewardConfig.BombSlice : default;

        public bool HasValidBaseline()
        {
            return _rewardConfig != null && _rewardConfig.HasValidBaseline();
        }
    }
}
