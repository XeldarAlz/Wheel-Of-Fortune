using System.Collections;
using UnityEngine;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.View.Audio
{
    [RequireComponent(typeof(AudioListener))]
    public sealed class AudioPlayer : MonoBehaviour, IAudioPlayer
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        private Coroutine _musicFadeCoroutine;

        public void PlayMusic(AudioClip clip, bool loop)
        {
            if (_musicSource == null || clip == null)
            {
                return;
            }

            _musicSource.clip = clip;
            _musicSource.loop = loop;
            _musicSource.volume = 0.0f;
            _musicSource.Play();
        }

        public void StopMusic()
        {
            if (!_musicSource)
            {
                return;
            }

            _musicSource.Stop();
            _musicSource.clip = null;
        }

        public bool IsMusicPlaying()
        {
            return _musicSource != null && _musicSource.isPlaying;
        }

        public bool IsMusicPlaying(AudioClip clip)
        {
            return _musicSource != null && _musicSource.isPlaying && _musicSource.clip == clip;
        }

        public void FadeMusicTo(float targetVolume, float duration, bool stopOnComplete)
        {
            if (_musicSource == null)
            {
                return;
            }

            if (_musicFadeCoroutine != null)
            {
                StopCoroutine(_musicFadeCoroutine);
            }

            _musicFadeCoroutine = StartCoroutine(FadeMusicCoroutine(targetVolume, duration, stopOnComplete));
        }

        public void PlaySfxOneShot(AudioClip clip)
        {
            if (!_sfxSource || !clip)
            {
                return;
            }

            _sfxSource.PlayOneShot(clip);
        }

        public void PlaySfxLooped(AudioClip clip)
        {
            if (!_sfxSource || !clip)
            {
                return;
            }

            _sfxSource.clip = clip;
            _sfxSource.Play();
        }

        public void StopSfx()
        {
            if (_sfxSource == null)
            {
                return;
            }

            _sfxSource.Stop();
            _sfxSource.clip = null;
        }

        private IEnumerator FadeMusicCoroutine(float targetVolume, float duration, bool stopOnComplete)
        {
            float initialVolume = _musicSource.volume;
            float elapsed = 0.0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsed / duration);
                _musicSource.volume = Mathf.Lerp(initialVolume, targetVolume, t);
                yield return null;
            }

            _musicSource.volume = targetVolume;

            if (stopOnComplete)
            {
                StopMusic();
            }

            _musicFadeCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_musicFadeCoroutine != null)
            {
                StopCoroutine(_musicFadeCoroutine);
            }
        }
    }
}
