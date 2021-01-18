using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public static class MusicSortDeserialization
    {
        public static MusicListSortSelector<MusicFile> Deserialize(MusicListSortMembers member)
        {
            MusicListSortSelector<MusicFile> keySelector = null;

            switch (member)
            {
                case MusicListSortMembers.TrackId:
                    keySelector = s => s.TrackNumber;
                    break;
                case MusicListSortMembers.Name:
                    keySelector = s => s;
                    break;
                case MusicListSortMembers.ModifyTime:
                    keySelector = s => s.ModifyTime;
                    break;
            }

            return keySelector;
        }
    }
}