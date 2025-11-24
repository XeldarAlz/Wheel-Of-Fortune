using WheelOfFortune.Infrastructure.Interfaces;

namespace WheelOfFortune.Presentation.Presenters
{
    public sealed class BombPresenter : IBombPresenter
    {
        private readonly IBombView _view;
        private readonly IHapticFeedbackService _hapticService;
        private readonly IAudioService _audioService;
        private readonly IWheelGameService _gameService;
        private readonly IRevivalService _revivalService;
        private readonly int _reviveCost;

        public BombPresenter(IBombView view, IHapticFeedbackService hapticService, IAudioService audioService,
            IWheelGameService gameService, IRevivalService revivalService, int reviveCost)
        {
            _view = view;
            _hapticService = hapticService;
            _audioService = audioService;
            _gameService = gameService;
            _revivalService = revivalService;
            _reviveCost = reviveCost;

            SubscribeToView();
            UpdateReviveButtonState();
            UpdateReviveCostText();
        }

        private void SubscribeToView()
        {
            if (_view == null)
            {
                return;
            }

            _view.GiveUpRequested += HandleGiveUpRequested;
            _view.ReviveRequested += HandleReviveRequested;
        }

        public void ShowBombScreen()
        {
            _view?.Show();
            UpdateReviveButtonState();
            UpdateReviveCostText();
        }

        public void HideBombScreen()
        {
            _view?.Hide();
        }

        private void HandleGiveUpRequested()
        {
            _hapticService?.Light();
            _audioService?.PlayButtonGiveUp();

            if (_gameService != null)
            {
                _gameService.ResetToInitialState();
            }

            HideBombScreen();
        }

        private void HandleReviveRequested()
        {
            _hapticService?.Light();
            _audioService?.PlayButtonContinue();

            if (_revivalService == null || _gameService == null)
            {
                return;
            }

            int availableCurrency = _gameService.GetPendingCurrency();

            if (!_revivalService.CanRevive(_reviveCost, availableCurrency))
            {
                UpdateReviveButtonState();
                return;
            }

            if (_revivalService.TryRevive(_reviveCost, availableCurrency))
            {
                _gameService.SpendCurrency(_reviveCost);
                HideBombScreen();
            }

            UpdateReviveButtonState();
        }

        private void UpdateReviveButtonState()
        {
            bool canRevive = false;

            if (_revivalService != null && _gameService != null)
            {
                int availableCurrency = _gameService.GetPendingCurrency();
                canRevive = _revivalService.CanRevive(_reviveCost, availableCurrency);
            }

            _view?.UpdateReviveButtonState(canRevive);
        }

        private void UpdateReviveCostText()
        {
            _view?.UpdateReviveCostText(_reviveCost);
        }

        private void UnsubscribeFromView()
        {
            if (_view == null)
            {
                return;
            }

            _view.GiveUpRequested -= HandleGiveUpRequested;
            _view.ReviveRequested -= HandleReviveRequested;
        }
        
        public void Dispose()
        {
            UnsubscribeFromView();
        }
    }
}
