namespace WheelOfFortune.Core.Models
{
    public sealed class WheelRewardBank
    {
        private int _pendingReward;
        private int _lifetimeReward;

        public int PendingReward => _pendingReward;
        public int LifetimeReward => _lifetimeReward;

        public void AddReward(int value)
        {
            _pendingReward += value;
            _lifetimeReward += value;
        }

        public void ClearPending()
        {
            _pendingReward = 0;
        }

        public bool TrySpend(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (_pendingReward < amount)
            {
                return false;
            }

            _pendingReward -= amount;
            return true;
        }
    }
}
