using UnityEngine;

namespace WheelOfFortune.Data.Zone
{
    [CreateAssetMenu(fileName = "ZoneVisualConfig", menuName = "WheelOfFortune/Zone Visual Config")]
    public sealed class ZoneVisualConfig : ScriptableObject
    {
        [Header("Text")]
        [SerializeField] private string _titleText = string.Empty;
        [SerializeField] private string _statusText = string.Empty;
        [Header("Colors")]
        [SerializeField] private Color _textColor = Color.white;
        [SerializeField] private Color _backgroundColor = Color.white;
        [Header("Sprites")]
        [SerializeField] private Sprite _wheelSprite;
        [SerializeField] private Sprite _indicatorSprite;

        public string TitleText => _titleText;
        public string StatusText => _statusText;
        public Color TextColor => _textColor;
        public Color BackgroundColor => _backgroundColor;
        public Sprite WheelSprite => _wheelSprite;
        public Sprite IndicatorSprite => _indicatorSprite;
    }
}
