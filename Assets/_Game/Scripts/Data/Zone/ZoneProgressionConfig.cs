using UnityEngine;

namespace WheelOfFortune.Data.Zone
{
    [CreateAssetMenu(fileName = "ZoneProgressionConfig", menuName = "WheelOfFortune/Zone Progression Config")]
    public sealed class ZoneProgressionConfig : ScriptableObject
    {
        private const int DEFAULT_SAFE_ZONE_INTERVAL = 5;
        private const int DEFAULT_SUPER_ZONE_INTERVAL = 30;

        [Header("Zone Intervals")]
        [SerializeField] private int _safeZoneInterval = DEFAULT_SAFE_ZONE_INTERVAL;
        [SerializeField] private int _superZoneInterval = DEFAULT_SUPER_ZONE_INTERVAL;

        public int SafeZoneInterval => _safeZoneInterval;
        public int SuperZoneInterval => _superZoneInterval;
    }
}
