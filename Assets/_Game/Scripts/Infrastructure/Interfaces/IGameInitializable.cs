using WheelOfFortune.Core.Models;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IGameInitializable
    {
        void Initialize(GameContext gameContext);
    }
}