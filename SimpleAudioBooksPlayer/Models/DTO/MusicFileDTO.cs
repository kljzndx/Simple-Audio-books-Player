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

        public MusicFileDTO(MusicFile source)
        {
            Group = FileGroupDataServer.Current.Data.First(g => g.Index == source.GroupId);
            FileTrackNumber = source.TrackNumber;
            Title = source.Title;
            Duration = source.Duration;
            FileName = source.FileName;
            FilePath = source.FilePath;
            ModifyTime = source.ModifyTime;

            var matches = NumberRegex.Matches(Title);
            if (matches.Any(m => m.Success) && UInt64.TryParse(String.Concat(matches.Select(m => m.Value)), out ulong result))
                TitleTrackNumber = result;
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        public FileGroupDTO Group { get; }

        public uint FileTrackNumber { get; }
        public ulong TitleTrackNumber { get; }
        public string Title { get; }
        public TimeSpan Duration { get; }

        public string FileName { get; }
        public string FilePath { get; }
        public DateTime ModifyTime { get; }

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
            var file = await GetFile();

            if (_playbackItem is null)
                _playbackItem = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));

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