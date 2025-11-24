using WheelOfFortune.Core.Models;
using WheelOfFortune.Core.StateMachine;
using WheelOfFortune.Core.StateMachine.States;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Core.Services
{
    public sealed class RevivalService : IRevivalService
    {
        private readonly WheelGameModel _model;
        private readonly WheelGameStateMachine _stateMachine;
        private readonly PlayingState _playingState;

        public RevivalService(WheelGameModel model, WheelGameStateMachine stateMachine, PlayingState playingState)
        {
            _model = model;
            _stateMachine = stateMachine;
            _playingState = playingState;
        }

        public bool CanRevive(int cost, int availableCurrency)
        {
            return availableCurrency >= cost;
        }

        public bool TryRevive(int cost, int availableCurrency)
        {
            if (!CanRevive(cost, availableCurrency))
            {
                return false;
            }

            if (!_model.TrySpendCurrency(cost))
            {
                return false;
            }

            _model.SetPendingBomb(false);
            _stateMachine.ChangeState(_playingState);
            return true;
        }
    }
}
