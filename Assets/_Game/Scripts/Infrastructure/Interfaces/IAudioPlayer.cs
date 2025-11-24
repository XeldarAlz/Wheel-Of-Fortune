using UnityEngine;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IAudioPlayer
    {
        void PlayMusic(AudioClip clip, bool loop);
        void StopMusic();
        bool IsMusicPlaying();
        bool IsMusicPlaying(AudioClip clip);
        void FadeMusicTo(float targetVolume, float duration, bool stopOnComplete);
        void PlaySfxOneShot(AudioClip clip);
        void PlaySfxLooped(AudioClip clip);
        void StopSfx();
    }
}

