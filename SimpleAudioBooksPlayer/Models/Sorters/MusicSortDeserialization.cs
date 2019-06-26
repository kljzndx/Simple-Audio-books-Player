using System;
using System.Text.RegularExpressions;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public enum MusicListSortMembers
    {
        TrackId,
        Name,
        ModifyTime
    }

    public delegate IComparable MusicListSortSelector<T>(T source);

    public static class MusicSortDeserialization
    {
        public static MusicListSortSelector<MusicFileDTO> Deserialize(MusicListSortMembers member)
        {
            MusicListSortSelector<MusicFileDTO> keySelector = null;

            switch (member)
            {
                case MusicListSortMembers.TrackId:
                    keySelector = s => s.FileTrackNumber;
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