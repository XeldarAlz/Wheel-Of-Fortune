using UnityEngine;
using WheelOfFortune.Core.Services;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;

namespace WheelOfFortune.Presentation.Installers
{
    [RequireComponent(typeof(IHapticPlayer))]
    public sealed class HapticFeedbackServiceInstaller : MonoBehaviour
    {
        private HapticFeedbackService _hapticService;

        private void Awake()
        {
            CreateService();
            RegisterService();
        }

        private void CreateService()
        {
            IHapticPlayer hapticPlayer = GetComponent<IHapticPlayer>();
            _hapticService = new HapticFeedbackService(hapticPlayer);
        }

        private void RegisterService()
        {
            ServiceLocator.Instance.Register<IHapticFeedbackService>(_hapticService);
        }

        private void UnregisterService()
        {
            ServiceLocator.Instance.Unregister<IHapticFeedbackService>();
        }
        
        private void OnDestroy()
        {
            UnregisterService();
        }
    }
}
