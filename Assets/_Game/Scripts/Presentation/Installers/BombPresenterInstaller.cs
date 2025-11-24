using UnityEngine;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Presentation.Presenters;
using WheelOfFortune.View.Components;

namespace WheelOfFortune.Presentation.Installers
{
    [RequireComponent(typeof(BombView))]
    public sealed class BombPresenterInstaller : MonoBehaviour
    {
        [SerializeField][Range(0, 300)] private int _reviveCost = 75;

        private BombPresenter _presenter;

        private void Start()
        {
            CreatePresenter();
            RegisterPresenter();
        }

        private void CreatePresenter()
        {
            IBombView view = GetComponent<IBombView>();

            if (view == null)
            {
                return;
            }

            IHapticFeedbackService hapticService = ServiceLocator.Instance.Get<IHapticFeedbackService>();
            IAudioService audioService = ServiceLocator.Instance.Get<IAudioService>();
            IWheelGameService gameService = ServiceLocator.Instance.Get<IWheelGameService>();
            IRevivalService revivalService = ServiceLocator.Instance.Get<IRevivalService>();

            _presenter = new BombPresenter(view, hapticService, audioService, gameService, revivalService, _reviveCost);
        }

        private void RegisterPresenter()
        {
            ServiceLocator.Instance.Register<IBombPresenter>(_presenter);
        }

        private void UnregisterPresenter()
        {
            ServiceLocator.Instance.Unregister<IBombPresenter>();
        }

        private void DisposePresenter()
        {
            _presenter?.Dispose();
        }

        private void OnDestroy()
        {
            UnregisterPresenter();
            DisposePresenter();
        }
    }
}
