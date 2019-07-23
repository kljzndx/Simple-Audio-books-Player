using System;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class OtherSettingProperties : SettingsBase
    {
        public static readonly OtherSettingProperties Current = new OtherSettingProperties();

        [SettingFieldByNormal(nameof(IsCreatedCoverFolder), false)] private bool _isCreatedCoverFolder;

        private OtherSettingProperties() : base("Other")
        {
        }

        public bool IsCreatedCoverFolder
        {
            get => _isCreatedCoverFolder;
            set => SetSetting(ref _isCreatedCoverFolder, value);
        }

        public DateTime ExitTime { get; set; }
    }
}