using System;
using System.Linq;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class PlaybackRecordDTO : ObservableObject
    {
        private string _currentTitle;
        private DateTime _playDate;
        private TimeSpan _playedTime;

        public PlaybackRecordDTO(string currentTitle, FileGroupDTO @group, uint trackId, MusicListSortMembers sortMethod, bool isReverse, TimeSpan playedTime)
        {
            _currentTitle = currentTitle;
            Group = @group;
            TrackId = trackId;
            SortMethod = sortMethod;
            IsReverse = isReverse;
            _playDate = DateTime.Now;
            PlayedTime = playedTime;
        }

        public PlaybackRecordDTO(string currentTitle, FileGroupDTO @group, uint trackId, MusicListSortMembers sortMethod, bool isReverse) : this(currentTitle, group, trackId, sortMethod, isReverse, TimeSpan.Zero)
        {
        }

        public PlaybackRecordDTO(PlaybackRecord source)
        {
            Group = FileGroupDataServer.Current.Data.First(g => g.Index == source.GroupId);
            TrackId = source.TrackId;
            CurrentTitle = source.CurrentTitle;
            SortMethod = Enum.Parse<MusicListSortMembers>(source.SortMethod);
            IsReverse = source.IsReverse;
            _playDate = source.PlayDate;
            PlayedTime = source.PlayedTime;
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

        public DateTime PlayDate
        {
            get => _playDate;
            set => Set(ref _playDate, value);
        }

        public TimeSpan PlayedTime
        {
            get => _playedTime;
            set => Set(ref _playedTime, value);
        }

        public PlaybackRecord ToTableObject()
        {
            return new PlaybackRecord(Group.Index, TrackId, CurrentTitle, SortMethod.ToString(), IsReverse, PlayDate, PlayedTime);
        }

        public void Update(PlaybackRecordDTO source)
        {
            if (!Group.Equals(source.Group))
                throw new Exception("新的播放记录不是当前组的播放记录");

            TrackId = source.TrackId;
            CurrentTitle = source.CurrentTitle;
            SortMethod = source.SortMethod;
            IsReverse = source.IsReverse;
            PlayDate = DateTime.Now;
            PlayedTime = source.PlayedTime;
        }
    }
}