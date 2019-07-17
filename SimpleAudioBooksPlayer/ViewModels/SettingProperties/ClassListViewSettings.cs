using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public class ClassListViewSettings : SettingsBase
    {
        public static readonly ClassListViewSettings Current = new ClassListViewSettings();

        [SettingFieldByNormal(nameof(ListWidth), 220D)] private double _listWidth;

        private ClassListViewSettings() : base("ClassListView")
        {
        }

        public double ListWidth
        {
            get => _listWidth;
            set => SetSetting(ref _listWidth, value);
        }
    }
}