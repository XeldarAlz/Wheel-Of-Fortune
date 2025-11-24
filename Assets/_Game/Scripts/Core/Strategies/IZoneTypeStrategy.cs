using UnityEngine;
using WheelOfFortune.Data.Rewards;

namespace WheelOfFortune.Core.Strategies
{
    public interface IZoneTypeStrategy
    {
        Sprite GetWheelSprite();
        Sprite GetIndicatorSprite();
        WheelSliceConfig[] GetSlices();
        string GetTitleText();
        string GetStatusText();
        Color GetTextColor();
        Color GetBackgroundColor();
    }
}