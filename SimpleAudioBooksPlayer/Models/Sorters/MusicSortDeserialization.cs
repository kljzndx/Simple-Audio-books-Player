using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public static class MusicSortDeserialization
    {
        public static MusicListSortSelector<MusicFileDTO> Deserialize(MusicListSortMembers member)
        {
            MusicListSortSelector<MusicFileDTO> keySelector = null;

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