using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;
using SimpleAudioBooksPlayer.Models.Sorters;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public enum MusicListSortMembers
    {
        TrackId,
        Name,
        ModifyTime
    }

    public class MusicListSettingProperties : SettingsBase
    {
        public static MusicListSettingProperties Current = new MusicListSettingProperties();

        private MusicListSettingProperties() : base("MusicListView")
        {
        }

        [SettingFieldByEnum(nameof(SortMethod), typeof(MusicListSortMembers), nameof(MusicListSortMembers.Name))]
        private MusicListSortMembers _sortMethod;

        public MusicListSortMembers SortMethod
        {
            get => _sortMethod;
            set => SetSetting(ref _sortMethod, value);
        }

        [SettingFieldByNormal(nameof(IsReverse), false)] private bool _isReverse;

        public bool IsReverse
        {
            get => _isReverse;
            set => SetSetting(ref _isReverse, value);
        }
    }
}