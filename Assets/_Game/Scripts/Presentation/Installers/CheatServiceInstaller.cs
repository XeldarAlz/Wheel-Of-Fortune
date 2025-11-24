using UnityEngine;
using WheelOfFortune.Core.Services;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Presentation.Installers
{
    public sealed class CheatServiceInstaller : MonoBehaviour
    {
        private CheatService _cheatService;

        private void Start()
        {
            CreateService();
            RegisterService();
        }

        private void CreateService()
        {
            IWheelGameService gameService = ServiceLocator.Instance.Get<IWheelGameService>();
            ISpinRandomizer spinRandomizer = ServiceLocator.Instance.Get<ISpinRandomizer>();

            if (gameService == null)
            {
                return;
            }

            if (spinRandomizer == null)
            {
                return;
            }

            _cheatService = new CheatService(gameService, spinRandomizer);
        }

        private void RegisterService()
        {
            if (_cheatService == null)
            {
                return;
            }

            ServiceLocator.Instance.Register<ICheatService>(_cheatService);
        }

        private void UnregisterService()
        {
            ServiceLocator.Instance.Unregister<ICheatService>();
        }

        private void OnDestroy()
        {
            UnregisterService();
        }
    }
}
