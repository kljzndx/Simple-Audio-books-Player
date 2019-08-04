using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class PlaybackViewSettingProperties : SettingsBase
    {
        public static readonly PlaybackViewSettingProperties Current = new PlaybackViewSettingProperties();

        [SettingFieldByNormal(nameof(ListWidth), 240D)] private double _listWidth;

        private PlaybackViewSettingProperties() : base("PlaybackView")
        {
        }

        public double ListWidth
        {
            get => _listWidth;
            set => SetSetting(ref _listWidth, value);
        }
    }
}