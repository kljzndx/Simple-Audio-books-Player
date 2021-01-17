using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Attributes;

namespace SimpleAudioBooksPlayer.ViewModels.SettingProperties
{
    public enum GroupListSorterMember
    {
        Name,
        CreateTime
    }

    public class GroupListViewSettings : SettingsBase
    {
        public static readonly GroupListViewSettings Current = new GroupListViewSettings();

        [SettingFieldByEnum(nameof(SortMethod), typeof(GroupListSorterMember), nameof(GroupListSorterMember.Name))] private GroupListSorterMember _sortMethod;
        [SettingFieldByNormal(nameof(IsReverse), false)] private bool _isReverse;
        [SettingFieldByNormal(nameof(IsCheckCover), false)] private bool _isCheckCover;

        private GroupListViewSettings() : base("GroupView")
        {
        }

        public GroupListSorterMember SortMethod
        {
            get => _sortMethod;
            set => SetSetting(ref _sortMethod, value);
        }

        public bool IsReverse
        {
            get => _isReverse;
            set => SetSetting(ref _isReverse, value);
        }

        public bool IsCheckCover
        {
            get => _isCheckCover;
            set => SetSetting(ref _isCheckCover, value);
        }
    }
}