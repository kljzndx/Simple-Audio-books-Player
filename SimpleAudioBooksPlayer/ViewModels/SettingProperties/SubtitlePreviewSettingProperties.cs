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
    }
}