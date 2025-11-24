using UnityEngine;

namespace WheelOfFortune.Data.Zone
{
    public readonly struct ZoneDisplayData
    {
        private readonly int _zoneIndex;
        private readonly string _titleText;
        private readonly string _statusText;
        private readonly Color _textColor;
        private readonly Color _backgroundColor;

        public int ZoneIndex => _zoneIndex;
        public string TitleText => _titleText;
        public string StatusText => _statusText;
        public Color TextColor => _textColor;
        public Color BackgroundColor => _backgroundColor;

        public ZoneDisplayData(int zoneIndex, string titleText, string statusText, Color textColor, Color backgroundColor)
        {
            _zoneIndex = zoneIndex;
            _titleText = titleText;
            _statusText = statusText;
            _textColor = textColor;
            _backgroundColor = backgroundColor;
        }
    }
}
