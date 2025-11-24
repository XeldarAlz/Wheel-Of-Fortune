namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IHapticFeedbackService
    {
        void Light();
        void Medium();
        void Heavy();
        void Warning();
        void Success();
        void Error();
        void StartContinuousTicks();
        void StopContinuousTicks();
    }
}