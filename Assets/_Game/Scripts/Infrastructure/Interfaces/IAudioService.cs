namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IAudioService
    {
        void PlayBackgroundLoop();
        void StopBackgroundLoop();
        void PlayWheelRotate();
        void StopWheelRotate();
        void PlayWheelReward();
        void PlayBombHit();
        void PlayButtonLeave();
        void PlayButtonGiveUp();
        void PlayButtonContinue();
        void PlaySilverZoneEnter();
        void PlaySuperZoneEnter();
    }
}