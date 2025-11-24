using UnityEngine;
using WheelOfFortune.Data.Rewards;
using WheelOfFortune.Data.Zone;

namespace WheelOfFortune.Infrastructure.Interfaces
{
    public interface IWheelView
    {
        void RenderSlices(WheelSliceConfig[] sliceConfigs);
        void ShowZone(ZoneDisplayData zoneData, bool animateTransition);
        void SetWheelVisuals(Sprite wheelSprite, Sprite pointerSprite, bool animateTransition);
        void SetButtonsInteractable(bool canSpin, bool canLeave);
        void ShowSliceStarEffect(int sliceIndex);
        void HideAllSliceStarEffects();
    }
}