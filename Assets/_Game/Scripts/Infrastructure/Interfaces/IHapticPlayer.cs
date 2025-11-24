namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IHapticPlayer
    {
        void TriggerHaptic(long durationMs);
        void PlayWarningPattern();
        void PlaySuccessPattern();
        void PlayErrorPattern();
        void StartContinuousTicks();
        void StopContinuousTicks();
    }
}