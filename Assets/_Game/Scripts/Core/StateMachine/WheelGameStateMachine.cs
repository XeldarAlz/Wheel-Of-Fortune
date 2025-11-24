namespace WheelOfFortune.Core.StateMachine
{
    public sealed class WheelGameStateMachine
    {
        private IWheelGameState _currentState;

        public void ChangeState(IWheelGameState newState)
        {
            _currentState?.Exit();
            _currentState = newState;
            _currentState?.Enter();
        }

        public void HandleSpinRequested()
        {
            _currentState?.HandleSpinRequested();
        }

        public void HandleLeaveRequested()
        {
            _currentState?.HandleLeaveRequested();
        }

        public void HandleContinueRequested()
        {
            _currentState?.HandleContinueRequested();
        }

        public void HandleGiveUpRequested()
        {
            _currentState?.HandleGiveUpRequested();
        }
    }
}
