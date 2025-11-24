namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface ICheatService
    {
        void ForceNextSlice(int sliceIndex);
        void ClearForcedSlice();
        bool TryForceBombSlice();
        bool TryForceRandomNonBombSlice();
        void SetBombChance(float chance);
        void SetSeed(int seed);
        void UseRandomSeed();
    }
}