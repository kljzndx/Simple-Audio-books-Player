using SimpleAudioBooksPlayer.Models.DTO;

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
                    keySelector = s => s.Title;
                    break;
                case MusicListSortMembers.ModifyTime:
                    keySelector = s => s.ModifyTime;
                    break;
            }

            return keySelector;
        }
    }
}