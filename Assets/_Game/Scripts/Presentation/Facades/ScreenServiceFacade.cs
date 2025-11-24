using UnityEngine;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;

namespace WheelOfFortune.Presentation.Facades
{
    public sealed class ScreenServiceFacade : MonoBehaviour, IScreenService
    {
        [SerializeField] private GameObject _wheelScreen;
        [SerializeField] private GameObject _bombScreen;

        private IWheelGameService _gameService;
        private IBombPresenter _bombPresenter;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IScreenService>(this);
        }

        public void ShowWheelScreen()
        {
            if (_wheelScreen)
            {
                _wheelScreen.SetActive(true);
            }

            if (_bombScreen)
            {
                _bombScreen.SetActive(false);
            }
        }

        public void ShowBombScreen()
        {
            if (_wheelScreen)
            {
                _wheelScreen.SetActive(false);
            }

            if (_bombScreen)
            {
                _bombScreen.SetActive(true);
            }

            ResolveBombPresenter();
            _bombPresenter?.ShowBombScreen();
        }

        private void ResolveBombPresenter()
        {
            ServiceResolver.Resolve(ref _bombPresenter);
        }
        
        private void OnDestroy()
        {
            ServiceLocator.Instance.Unregister<IScreenService>();
        }
    }
}
