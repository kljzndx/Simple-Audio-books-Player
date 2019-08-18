using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class PlayerSettingProperties : SettingsBase
    {
        public static readonly PlayerSettingProperties Current = new PlayerSettingProperties();

        [SettingFieldByNormal(nameof(Volume), 1D)] private double volume;
        [SettingFieldByNormal(nameof(PlaybackRate), 1D)] private double playbackRate;

        [SettingFieldByNormal(nameof(SingleLoopingModeEnable), false)] private bool _singleLoopingModeEnable;
        [SettingFieldByNormal(nameof(LoopingTimes), 0D)] private double _loopingTimes;

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

        public bool SingleLoopingModeEnable
        {
            get => _singleLoopingModeEnable;
            set => SetSetting(ref _singleLoopingModeEnable, value);
        }

        public double LoopingTimes
        {
            get => _loopingTimes;
            set => SetSetting(ref _loopingTimes, value);
        }
    }
}