using WheelOfFortune.Core.Models;
using WheelOfFortune.Data.Rewards;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IWheelGameService
    {
        void RequestSpin();
        void RequestLeave();
        void RequestContinue();
        void RequestGiveUp();
        void ClearCollectedRewards();
        void ForceNextSlice(int sliceIndex);
        void ClearForcedSlice();
        void ResetToInitialState();
        void SetZoneToBronze();
        void SetZoneToSilver();
        void SetZoneToGold();
        int GetPendingCurrency();
        void SpendCurrency(int amount);
        bool CanSpin();
        bool CanLeave();
        bool CanContinue();
        SpinRequest GetActiveSpinRequest();
        WheelSliceConfig[] GetWorkingSlices();
    }
}