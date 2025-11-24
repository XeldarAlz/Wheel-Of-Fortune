using UnityEngine;
using WheelOfFortune.Core.Models;
using WheelOfFortune.Core.Services;
using WheelOfFortune.Core.StateMachine;
using WheelOfFortune.Core.StateMachine.States;
using WheelOfFortune.Data.Profiles;
using WheelOfFortune.Infrastructure.Interfaces;
using WheelOfFortune.Infrastructure.DependencyInjection;
using WheelOfFortune.Presentation.Presenters;
using WheelOfFortune.Presentation.Adapters;
using WheelOfFortune.View.Components;

namespace WheelOfFortune.Presentation.Installers
{
    public sealed class WheelGamePresenterInstaller : MonoBehaviour, IGameInitializable
    {
        private WheelGamePresenter _presenter;
        private RevivalService _revivalService;
        private bool _isInitialized;

        private void Awake()
        {
            ServiceLocator.Instance.Register<IGameInitializable>(this);
        }

        public void Initialize(GameContext gameContext)
        {
            if (_isInitialized)
            {
                return;
            }

            if (gameContext == null)
            {
                return;
            }

            CreatePresenter(gameContext);
            RegisterServices();
            
            _isInitialized = true;
            
            _presenter?.StartGame();
        }

        private void CreatePresenter(GameContext gameContext)
        {
            WheelGameModel model = gameContext.Model;
            WheelGameStateMachine stateMachine = gameContext.StateMachine;
            PlayingState playingState = gameContext.PlayingState;
            BombState bombState = gameContext.BombState;
            WheelProgressionProfile progressionProfile = gameContext.ProgressionProfile;

            IWheelSpinService spinService = ServiceLocator.Instance.Get<IWheelSpinService>();
            IWheelView view = ServiceLocator.Instance.Get<IWheelView>();
            IHapticFeedbackService hapticService = ServiceLocator.Instance.Get<IHapticFeedbackService>();
            ISpinRandomizer spinRandomizer = ServiceLocator.Instance.Get<ISpinRandomizer>();
            IAudioService audioService = ServiceLocator.Instance.Get<IAudioService>();

            CollectedRewardsPresenter collectedRewardsPresenter = CreateCollectedRewardsPresenter(view);
            
            _revivalService = new RevivalService(model, stateMachine, playingState);

            _presenter = new WheelGamePresenter(model, stateMachine, playingState, bombState, progressionProfile,
                collectedRewardsPresenter, spinService, view, hapticService, spinRandomizer, audioService,
                _revivalService);
        }

        private CollectedRewardsPresenter CreateCollectedRewardsPresenter(IWheelView viewHud)
        {
            if (viewHud is not WheelViewAdapter adapter)
            {
                return null;
            }

            WheelView view = adapter.View;

            if (view == null)
            {
                return null;
            }

            if (view.RewardItemPrefab == null || view.CollectedRewardsContainer == null)
            {
                return null;
            }

            return new CollectedRewardsPresenter(view.RewardItemPrefab, view.CollectedRewardsContainer,
                view.CollectedRewardsLabel);
        }

        private void RegisterServices()
        {
            ServiceLocator.Instance.Register<IWheelGameService>(_presenter);
            ServiceLocator.Instance.Register<IRevivalService>(_revivalService);
        }

        private void UnregisterServices()
        {
            ServiceLocator.Instance.Unregister<IGameInitializable>();
            ServiceLocator.Instance.Unregister<IWheelGameService>();
            ServiceLocator.Instance.Unregister<IRevivalService>();
        }

        private void DisposePresenter()
        {
            _presenter?.Dispose();
        }

        private void OnDestroy()
        {
            UnregisterServices();
            DisposePresenter();
        }
    }
}
