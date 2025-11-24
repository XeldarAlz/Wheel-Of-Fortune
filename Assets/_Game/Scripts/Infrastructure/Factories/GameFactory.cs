using WheelOfFortune.Core.Models;
using WheelOfFortune.Core.StateMachine;
using WheelOfFortune.Core.StateMachine.States;
using WheelOfFortune.Data.Profiles;

namespace WheelOfFortune.Infrastructure.Factories
{
    public sealed class GameFactory
    {
        private readonly WheelProgressionProfile _progressionProfile;

        public GameFactory(WheelProgressionProfile progressionProfile)
        {
            _progressionProfile = progressionProfile;
        }

        public GameContext CreateGameContext()
        {
            int safeZoneInterval = _progressionProfile.SafeZoneInterval;
            int superZoneInterval = _progressionProfile.SuperZoneInterval; 
            
            WheelGameModel model = new(safeZoneInterval, superZoneInterval);
            WheelGameStateMachine stateMachine = new();
            PlayingState playingState = new(model, _progressionProfile);
            BombState bombState = new(model, stateMachine, playingState);

            return new GameContext(model, stateMachine, playingState, bombState, _progressionProfile);
        }
    }
}
