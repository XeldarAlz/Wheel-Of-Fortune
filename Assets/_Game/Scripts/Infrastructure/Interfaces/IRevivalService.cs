namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IRevivalService
    {
        bool CanRevive(int cost, int availableCurrency);
        bool TryRevive(int cost, int availableCurrency);
    }
}