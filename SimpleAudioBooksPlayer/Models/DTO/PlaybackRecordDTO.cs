
using System;
using System.Linq;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class PlaybackRecordDTO : ObservableObject
    {
        private string _currentTitle;

        public PlaybackRecordDTO(string currentTitle, FileGroupDTO @group, uint trackId, MusicListSortMembers sortMethod, bool isReverse)
        {
            _currentTitle = currentTitle;
            Group = @group;
            TrackId = trackId;
            SortMethod = sortMethod;
            IsReverse = isReverse;
        }

        public PlaybackRecordDTO(PlaybackRecord source)
        {
            Group = FileGroupDataServer.Current.Data.First(g => g.Index == source.GroupId);
            TrackId = source.TrackId;
            CurrentTitle = source.CurrentTitle;
            SortMethod = Enum.Parse<MusicListSortMembers>(source.SortMethod);
            IsReverse = source.IsReverse;
        }

        public FileGroupDTO Group { get; set; }
        public uint TrackId { get; set; }
        public string CurrentTitle
        {
            get => _currentTitle;
            set => Set(ref _currentTitle, value);
        }
        public MusicListSortMembers SortMethod { get; set; }
        public bool IsReverse { get; set; }

        public PlaybackRecord ToTableObject()
        {
            return new PlaybackRecord(Group.Index, TrackId, CurrentTitle, SortMethod.ToString(), IsReverse);
        }
    }
}