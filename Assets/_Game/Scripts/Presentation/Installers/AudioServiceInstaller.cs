using UnityEngine;
using WheelOfFortune.Core.Services;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Presentation.Installers
{
    [RequireComponent(typeof(IAudioPlayer))]
    public sealed class AudioServiceInstaller : MonoBehaviour
    {
        [Header("Music")]
        [SerializeField] private AudioClip _backgroundLoopClip;
        [Header("Zone")]
        [SerializeField] private AudioClip _zoneChangeSilverClip;
        [SerializeField] private AudioClip _zoneChangeGoldClip;
        [Header("Wheel")]
        [SerializeField] private AudioClip _wheelRotateClip;
        [SerializeField] private AudioClip _wheelRewardClip;
        [SerializeField] private AudioClip _bombExplodeClip;
        [Header("UI")]
        [SerializeField] private AudioClip _buttonLeaveClip;
        [SerializeField] private AudioClip _buttonGiveUpClip;
        [SerializeField] private AudioClip _buttonReviveClip;
        [Header("Music Settings")]
        [SerializeField] [Range(0.0f, 1.0f)] private float _musicMaxVolume = 0.15f;

        private AudioService _audioService;

        private void Awake()
        {
            CreateService();
            RegisterService();
        }

        private void CreateService()
        {
            IAudioPlayer audioPlayer = GetComponent<IAudioPlayer>();

            _audioService = new AudioService(audioPlayer, _backgroundLoopClip, _wheelRotateClip, _wheelRewardClip,
                _bombExplodeClip, _buttonLeaveClip, _buttonGiveUpClip, _buttonReviveClip, _zoneChangeSilverClip,
                _zoneChangeGoldClip, _musicMaxVolume);
        }

        private void RegisterService()
        {
            ServiceLocator.Instance.Register<IAudioService>(_audioService);
        }

        private void UnregisterService()
        {
            ServiceLocator.Instance.Unregister<IAudioService>();
        }

        private void OnDestroy()
        {
            UnregisterService();
        }
    }
}
