namespace WheelOfFortune.Core.StateMachine
{
    public interface IWheelGameState
    {
        void Enter();
        void Exit();
        void HandleSpinRequested();
        void HandleLeaveRequested();
        void HandleContinueRequested();
        void HandleGiveUpRequested();
    }
}