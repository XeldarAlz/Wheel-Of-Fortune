using WheelOfFortune.Core.Models;
using WheelOfFortune.Data.Profiles;
using WheelOfFortune.Data.Zone;

namespace WheelOfFortune.Core.Strategies
{
    public sealed class ZoneStrategyFactory
    {
        private readonly BronzeZoneStrategy _bronzeStrategy;
        private readonly SilverZoneStrategy _silverStrategy;
        private readonly SuperZoneStrategy _superStrategy;

        public ZoneStrategyFactory(WheelProgressionProfile profile)
        {
            ZoneVisualConfig bronzeVisual = profile != null 
                ? profile.BronzeVisualConfig 
                : null;
            ZoneVisualConfig silverVisual = profile != null 
                ? profile.SilverVisualConfig 
                : null;
            ZoneVisualConfig superVisual = profile != null 
                ? profile.SuperVisualConfig 
                : null;

            _bronzeStrategy = new BronzeZoneStrategy(profile, bronzeVisual);
            _silverStrategy = new SilverZoneStrategy(profile, silverVisual);
            _superStrategy = new SuperZoneStrategy(profile, superVisual);
        }

        public IZoneTypeStrategy GetStrategy(WheelGameModel model)
        {
            if (model.IsSuperZone)
            {
                return _superStrategy;
            }

            if (model.IsSafeZone)
            {
                return _silverStrategy;
            }

            return _bronzeStrategy;
        }
    }
}
