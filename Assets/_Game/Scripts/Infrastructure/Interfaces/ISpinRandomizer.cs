using WheelOfFortune.Data.Rewards;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface ISpinRandomizer
    {
        int ResolveSliceIndex(WheelSliceConfig[] slices, int currentZoneIndex);
        void ForceNextSlice(int sliceIndex);
        void ClearForcedSlice();
        void SetBombChance(float chance);
        void SetSeed(int seed);
        void UseRandomSeed();
    }
}