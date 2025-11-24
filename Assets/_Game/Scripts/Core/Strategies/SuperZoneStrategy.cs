using UnityEngine;
using WheelOfFortune.Data.Profiles;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Zone;

namespace WheelOfFortune.Core.Strategies
{
    public sealed class SuperZoneStrategy : IZoneTypeStrategy
    {
        private readonly WheelProgressionProfile _profile;
        private readonly ZoneVisualConfig _visualConfig;

        public SuperZoneStrategy(WheelProgressionProfile profile, ZoneVisualConfig visualConfig)
        {
            _profile = profile;
            _visualConfig = visualConfig;
        }

        public Sprite GetWheelSprite()
        {
            return _visualConfig != null 
                ? _visualConfig.WheelSprite 
                : null;
        }

        public Sprite GetIndicatorSprite()
        {
            return _visualConfig != null 
                ? _visualConfig.IndicatorSprite 
                : null;
        }

        public WheelSliceConfig[] GetSlices()
        {
            return _profile?.SuperSlices;
        }

        public string GetTitleText()
        {
            return _visualConfig != null 
                ? _visualConfig.TitleText 
                : string.Empty;
        }

        public string GetStatusText()
        {
            return _visualConfig != null 
                ? _visualConfig.StatusText 
                : string.Empty;
        }

        public Color GetTextColor()
        {
            return _visualConfig != null 
                ? _visualConfig.TextColor 
                : Color.white;
        }

        public Color GetBackgroundColor()
        {
            return _visualConfig != null 
                ? _visualConfig.BackgroundColor 
                : Color.white;
        }
    }
}
