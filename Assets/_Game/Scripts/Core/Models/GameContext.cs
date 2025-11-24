using WheelOfFortune.Core.StateMachine;
using WheelOfFortune.Core.StateMachine.States;
using WheelOfFortune.Data.Profiles;

namespace WheelOfFortune.Core.Models
{
    public sealed class GameContext
    {
        private readonly WheelGameModel _model;
        private readonly WheelGameStateMachine _stateMachine;
        private readonly PlayingState _playingState;
        private readonly BombState _bombState;
        private readonly WheelProgressionProfile _progressionProfile;

        public WheelGameModel Model => _model;
        public WheelGameStateMachine StateMachine => _stateMachine;
        public PlayingState PlayingState => _playingState;
        public BombState BombState => _bombState;
        public WheelProgressionProfile ProgressionProfile => _progressionProfile;

        public GameContext(WheelGameModel model, WheelGameStateMachine stateMachine, PlayingState playingState, 
            BombState bombState, WheelProgressionProfile progressionProfile)
        {
            _model = model;
            _stateMachine = stateMachine;
            _playingState = playingState;
            _bombState = bombState;
            _progressionProfile = progressionProfile;
        }
    }
}
