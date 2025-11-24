using UnityEngine;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Core.Services
{
    public sealed class AudioService : IAudioService
    {
        private const float MUSIC_FADE_DURATION_SECONDS = 5.0f;

        private readonly IAudioPlayer _audioPlayer;
        private readonly AudioClip _backgroundLoopClip;
        private readonly AudioClip _wheelRotateClip;
        private readonly AudioClip _wheelRewardClip;
        private readonly AudioClip _bombHitClip;
        private readonly AudioClip _buttonLeaveClip;
        private readonly AudioClip _buttonGiveUpClip;
        private readonly AudioClip _buttonReviveClip;
        private readonly AudioClip _silverZoneEnterClip;
        private readonly AudioClip _superZoneEnterClip;
        private readonly float _musicMaxVolume;

        private bool _isWheelRotateActive;

        public AudioService(IAudioPlayer audioPlayer, AudioClip backgroundLoopClip, AudioClip wheelRotateClip,
            AudioClip wheelRewardClip, AudioClip bombHitClip, AudioClip buttonLeaveClip, AudioClip buttonGiveUpClip,
            AudioClip buttonReviveClip, AudioClip silverZoneEnterClip, AudioClip superZoneEnterClip,
            float musicMaxVolume)
        {
            _audioPlayer = audioPlayer;
            _backgroundLoopClip = backgroundLoopClip;
            _wheelRotateClip = wheelRotateClip;
            _wheelRewardClip = wheelRewardClip;
            _bombHitClip = bombHitClip;
            _buttonLeaveClip = buttonLeaveClip;
            _buttonGiveUpClip = buttonGiveUpClip;
            _buttonReviveClip = buttonReviveClip;
            _silverZoneEnterClip = silverZoneEnterClip;
            _superZoneEnterClip = superZoneEnterClip;
            _musicMaxVolume = musicMaxVolume;
        }

        public void PlayBackgroundLoop()
        {
            if (_audioPlayer == null || _backgroundLoopClip == null)
            {
                return;
            }

            if (_audioPlayer.IsMusicPlaying(_backgroundLoopClip))
            {
                _audioPlayer.FadeMusicTo(_musicMaxVolume, MUSIC_FADE_DURATION_SECONDS, false);
                return;
            }

            _audioPlayer.PlayMusic(_backgroundLoopClip, true);
            _audioPlayer.FadeMusicTo(_musicMaxVolume, MUSIC_FADE_DURATION_SECONDS, false);
        }

        public void StopBackgroundLoop()
        {
            if (_audioPlayer == null)
            {
                return;
            }

            if (!_audioPlayer.IsMusicPlaying())
            {
                _audioPlayer.StopMusic();
                return;
            }

            _audioPlayer.FadeMusicTo(0.0f, MUSIC_FADE_DURATION_SECONDS, true);
        }

        public void PlayWheelRotate()
        {
            if (!_wheelRotateClip || _isWheelRotateActive)
            {
                return;
            }

            _audioPlayer?.PlaySfxLooped(_wheelRotateClip);
            _isWheelRotateActive = true;
        }

        public void StopWheelRotate()
        {
            if (!_isWheelRotateActive)
            {
                return;
            }

            _audioPlayer?.StopSfx();
            _isWheelRotateActive = false;
        }

        public void PlayWheelReward()
        {
            PlayOneShot(_wheelRewardClip);
        }

        public void PlayBombHit()
        {
            PlayOneShot(_bombHitClip);
        }

        public void PlayButtonLeave()
        {
            PlayOneShot(_buttonLeaveClip);
        }

        public void PlayButtonGiveUp()
        {
            PlayOneShot(_buttonGiveUpClip);
        }

        public void PlayButtonContinue()
        {
            PlayOneShot(_buttonReviveClip);
        }

        public void PlaySilverZoneEnter()
        {
            PlayOneShot(_silverZoneEnterClip);
        }

        public void PlaySuperZoneEnter()
        {
            PlayOneShot(_superZoneEnterClip);
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (!clip)
            {
                return;
            }

            _audioPlayer?.PlaySfxOneShot(clip);
        }
    }
}
