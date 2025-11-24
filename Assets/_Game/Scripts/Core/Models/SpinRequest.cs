namespace WheelOfFortune.Core.Models
{
    public sealed class SpinRequest
    {
        private readonly int _targetSliceIndex;

        public int TargetSliceIndex => _targetSliceIndex;

        public SpinRequest(int targetSliceIndex)
        {
            _targetSliceIndex = targetSliceIndex;
        }
    }
}
