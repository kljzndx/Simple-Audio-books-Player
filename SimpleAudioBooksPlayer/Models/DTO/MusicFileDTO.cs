using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class MusicFileDTO : ObservableObject, IComparable, IComparable<MusicFileDTO>
    {
        private static readonly Regex NumberRegex = new Regex(@"[0-9]+");

        private WeakReference<StorageFile> _file;
        private MediaPlaybackItem _playbackItem;

        private bool _isPlaying;

        private uint _fileTrackNumber;
        private string _title;
        private TimeSpan _duration;
        private DateTime _modifyTime;

        public MusicFileDTO(MusicFile source)
        {
            Group = FileGroupDataServer.Current.Data.First(g => g.Index == source.GroupId);
            _fileTrackNumber = source.TrackNumber;
            _title = source.Title;
            _duration = source.Duration;
            FileName = source.FileName;
            FilePath = source.FilePath;
            _modifyTime = source.ModifyTime;

            GetTitleTrackNumber();
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        public bool HasRead { get; private set; }

        public FileGroupDTO Group { get; }
        public uint FileTrackNumber
        {
            get => _fileTrackNumber;
            private set => Set(ref _fileTrackNumber, value);
        }
        public ulong TitleTrackNumber { get; private set; }
        public string Title
        {
            get => _title;
            private set
            {
                Set(ref _title, value);
                GetTitleTrackNumber();
            }
        }

        public TimeSpan Duration
        {
            get => _duration;
            private set => Set(ref _duration, value);
        }

        public string FileName { get; }
        public string FilePath { get; }
        public DateTime ModifyTime
        {
            get => _modifyTime;
            private set => Set(ref _modifyTime, value);
        }

        public void Update(MusicFile source)
        {
            if (source.FilePath != FilePath)
                throw new Exception("不是同一个文件");

            FileTrackNumber = source.TrackNumber;
            Title = source.Title;
            Duration = source.Duration;
            ModifyTime = source.ModifyTime;
        }

        private void GetTitleTrackNumber()
        {
            var matches = NumberRegex.Matches(Title);
            if (matches.Any(m => m.Success) &&
                UInt64.TryParse(String.Concat(matches.Select(m => m.Value)), out ulong result))
                TitleTrackNumber = result;
        }

        private async Task<StorageFile> GetFile()
        {
            StorageFile file = null;
            _file?.TryGetTarget(out file);

            if (file is null)
            {
                file = await StorageFile.GetFileFromPathAsync(FilePath);
                _file = new WeakReference<StorageFile>(file);
            }

            return file;
        }

        public async Task<MediaPlaybackItem> GetPlaybackItem()
        {
            if (HasRead)
                return _playbackItem;

            var file = await GetFile();

            if (_playbackItem is null)
                _playbackItem = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));

            HasRead = true;
            return _playbackItem;
        }

        public int CompareTo(MusicFileDTO other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            var titleTrackNumberComparison = TitleTrackNumber.CompareTo(other.TitleTrackNumber);
            if (titleTrackNumberComparison != 0) return titleTrackNumberComparison;
            return string.Compare(Title, other.Title, StringComparison.Ordinal);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(null, obj)) return 1;
            if (obj.GetType() != this.GetType()) return 1;
            return CompareTo((MusicFileDTO) obj);
        }
    }
}