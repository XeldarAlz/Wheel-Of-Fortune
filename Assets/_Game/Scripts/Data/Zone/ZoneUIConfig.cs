using UnityEngine;

namespace WheelOfFortune.Data.Zone
{
    [CreateAssetMenu(fileName = "ZoneUIConfig", menuName = "WheelOfFortune/Zone UI Config")]
    public sealed class ZoneUIConfig : ScriptableObject
    {
        [SerializeField] private ZoneVisualConfig _bronzeVisualConfig;
        [SerializeField] private ZoneVisualConfig _silverVisualConfig;
        [SerializeField] private ZoneVisualConfig _superVisualConfig;

        public ZoneVisualConfig BronzeVisualConfig => _bronzeVisualConfig;
        public ZoneVisualConfig SilverVisualConfig => _silverVisualConfig;
        public ZoneVisualConfig SuperVisualConfig => _superVisualConfig;
    }
}
