using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class SubtitlePreviewSettingProperties : SettingsBase
    {
        public static readonly SubtitlePreviewSettingProperties Current = new SubtitlePreviewSettingProperties();

        [SettingFieldByNormal(nameof(FontSize), 16D)] private double _fontSize;

        private SubtitlePreviewSettingProperties() : base("SubtitlePreview")
        {
        }

        public double FontSize
        {
            get => _fontSize;
            set => SetSetting(ref _fontSize, value);
        }

        [SettingFieldByNormal(nameof(FontOpacity), 0.4D)] private double _fontOpacity;

        public double FontOpacity
        {
            get => _fontOpacity;
            set => SetSetting(ref _fontOpacity, value);
        }
    }
}