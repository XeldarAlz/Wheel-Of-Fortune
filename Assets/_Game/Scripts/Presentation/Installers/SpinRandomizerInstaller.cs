using UnityEngine;
using WheelOfFortune.Core.Services;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Presentation.Installers
{
    public sealed class SpinRandomizerInstaller : MonoBehaviour
    {
        private const float MINIMUM_CHANCE = 0.0f;
        private const float MAXIMUM_CHANCE = 1.0f;

        [Header("Bomb Chance Progression")]
        [SerializeField] [Range(MINIMUM_CHANCE, MAXIMUM_CHANCE)] private float _baseBombChance;
        [SerializeField] [Range(MINIMUM_CHANCE, MAXIMUM_CHANCE)] private float _bombChanceIncrement = 0.02f;
        [SerializeField] [Range(MINIMUM_CHANCE, MAXIMUM_CHANCE)] private float _maximumBombChance = 0.3f;
        [SerializeField] private int _incrementIntervalZones = 5;
        [Header("Seed Settings")]
        [SerializeField] private bool _useCustomSeed;
        [SerializeField] private int _customSeed = 12345;

        private SpinRandomizerService _service;

        private void Awake()
        {
            CreateService();
            RegisterService();
        }

        private void CreateService()
        {
            _service = new SpinRandomizerService(_baseBombChance, _bombChanceIncrement, _maximumBombChance,
                _incrementIntervalZones, _useCustomSeed, _customSeed);
        }

        private void RegisterService()
        {
            ServiceLocator.Instance.Register<ISpinRandomizer>(_service);
        }

        private void UnregisterService()
        {
            ServiceLocator.Instance.Unregister<ISpinRandomizer>();
        }
        
        private void OnDestroy()
        {
            UnregisterService();
        }
    }
}
