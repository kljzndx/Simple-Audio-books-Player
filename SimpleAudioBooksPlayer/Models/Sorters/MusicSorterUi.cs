using System;

namespace SimpleAudioBooksPlayer.Models.Sorters
{
    public enum MusicListSortMembers
    {
        TrackId,
        Name,
        ModifyTime
    }

    public delegate IComparable MusicListSortSelector<T>(T source);

    public class MusicSorterUi<T>
    {
        public MusicSorterUi(string resourceKey, MusicListSortSelector<T> keySelector)
        {
            Name = StringResources.SorterMembers.GetString(resourceKey);
            KeySelector = keySelector;
        }

        public string Name { get; }
        public MusicListSortSelector<T> KeySelector { get; }
    }
}