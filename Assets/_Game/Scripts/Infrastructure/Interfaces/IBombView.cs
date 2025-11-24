using System;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IBombView
    {
        event Action GiveUpRequested;
        event Action ReviveRequested;

        void Show();
        void Hide();
        void UpdateReviveButtonState(bool canRevive);
        void UpdateReviveCostText(int cost);
    }
}