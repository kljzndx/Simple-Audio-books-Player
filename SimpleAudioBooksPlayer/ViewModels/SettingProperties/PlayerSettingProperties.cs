using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public enum PlaybackRepeatModeEnum
    {
        Single,
        List,
        Random
    }

    public class PlayerSettingProperties : SettingsBase
    {
        public static readonly PlayerSettingProperties Current = new PlayerSettingProperties();

        [SettingFieldByNormal(nameof(Volume), 1D)] private double volume;
        [SettingFieldByNormal(nameof(PlaybackRate), 1D)] private double playbackRate;

        private PlayerSettingProperties() : base(ApplicationData.Current.LocalSettings.CreateContainer("Player", ApplicationDataCreateDisposition.Always))
        {
        }

        public double Volume
        {
            get => volume;
            set => SetSetting(ref volume, value);
        }

        public double PlaybackRate
        {
            get => playbackRate;
            set => SetSetting(ref playbackRate, value);
        }
    }
}