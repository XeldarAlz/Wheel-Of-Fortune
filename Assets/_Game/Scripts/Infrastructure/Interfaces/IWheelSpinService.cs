using System;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IWheelSpinService
    {
        event Action<int> SpinCompleted;
        bool IsSpinning { get; }
        void SpinToSlice(int targetSliceIndex, int totalSlices);
    }
}