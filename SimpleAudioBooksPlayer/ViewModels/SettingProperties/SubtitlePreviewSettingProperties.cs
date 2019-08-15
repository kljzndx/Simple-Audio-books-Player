using System;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class SubtitlePreviewSettingProperties : SettingsBase
    {
        public static readonly SubtitlePreviewSettingProperties Current = new SubtitlePreviewSettingProperties();

        [SettingFieldByNormal(nameof(FontSize), 16D)] private double _fontSize;
        [SettingFieldByNormal(nameof(FontOpacity), 0.35D)] private double _fontOpacity;

        [SettingFieldByNormal(nameof(SplitSymbols), "")] private string _splitSymbols;

        [SettingFieldByNormal(nameof(IsRereadingModeEnable), false)] private bool _isRereadingModeEnable;
        [SettingFieldByNormal(nameof(RereadingTimes), 1D)] private double _rereadingTimes;

        private SubtitlePreviewSettingProperties() : base("SubtitlePreview")
        {
        }

        public double FontSize
        {
            get => _fontSize;
            set => SetSetting(ref _fontSize, value);
        }

        public double FontOpacity
        {
            get => _fontOpacity;
            set => SetSetting(ref _fontOpacity, value);
        }


        public string SplitSymbols
        {
            get => _splitSymbols;
            set => SetSetting(ref _splitSymbols, value);
        }


        public bool IsRereadingModeEnable
        {
            get => _isRereadingModeEnable;
            set => SetSetting(ref _isRereadingModeEnable, value);
        }

        public double RereadingTimes
        {
            get => _rereadingTimes;
            set => SetSetting(ref _rereadingTimes, value);
        }
    }
}