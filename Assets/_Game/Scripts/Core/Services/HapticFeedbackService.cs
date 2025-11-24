using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Core.Services
{
    public sealed class HapticFeedbackService : IHapticFeedbackService
    {
        private const long LIGHT_VIBRATION_MS = 10;
        private const long MEDIUM_VIBRATION_MS = 20;
        private const long HEAVY_VIBRATION_MS = 40;

        private readonly IHapticPlayer _hapticPlayer;

        public HapticFeedbackService(IHapticPlayer hapticPlayer)
        {
            _hapticPlayer = hapticPlayer;
        }

        public void Light()
        {
            _hapticPlayer?.TriggerHaptic(LIGHT_VIBRATION_MS);
        }

        public void Medium()
        {
            _hapticPlayer?.TriggerHaptic(MEDIUM_VIBRATION_MS);
        }

        public void Heavy()
        {
            _hapticPlayer?.TriggerHaptic(HEAVY_VIBRATION_MS);
        }

        public void Warning()
        {
            _hapticPlayer?.PlayWarningPattern();
        }

        public void Success()
        {
            _hapticPlayer?.PlaySuccessPattern();
        }

        public void Error()
        {
            _hapticPlayer?.PlayErrorPattern();
        }

        public void StartContinuousTicks()
        {
            _hapticPlayer?.StartContinuousTicks();
        }

        public void StopContinuousTicks()
        {
            _hapticPlayer?.StopContinuousTicks();
        }
    }
}
