using UnityEngine;
using WheelOfFortune.Core.Models;
using WheelOfFortune.Data.Profiles;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Infrastructure.Factories;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Infrastructure.Bootstrapping
{
    public sealed class GameBootstrapper : MonoBehaviour
    {
        [SerializeField] private WheelProgressionProfile _progressionProfile;

        private void Awake()
        {
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
            QualitySettings.vSyncCount = 0;
        }

        private void Start()
        {
            InitializeGame();
        }

        private void InitializeGame()
        {
            IGameInitializable gameInitializable = ServiceLocator.Instance.Get<IGameInitializable>();

            if (gameInitializable == null)  
            {
                return;
            }

            GameFactory factory = new(_progressionProfile);
            GameContext gameContext = factory.CreateGameContext();

            gameInitializable.Initialize(gameContext);
        }
    }
}
