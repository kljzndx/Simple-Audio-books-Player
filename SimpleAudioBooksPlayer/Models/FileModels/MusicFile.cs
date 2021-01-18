using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.Sorters;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    public class MusicFile : LibraryFileBase, IComparable, IComparable<MusicFile>
    {
        private WeakReference<MediaPlaybackItem> _playbackItem;
        private bool _isPlaying;

        public MusicFile(FileGroupDTO group, string fileName, DateTime modifyTime, uint trackNumber, string title, TimeSpan duration) : base(group, fileName)
        {
            TrackNumber = trackNumber;
            Title = title;
            Duration = duration;
            ModifyTime = modifyTime;
            
            if (String.IsNullOrWhiteSpace(Title))
                Title = DisplayName;
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        public uint TrackNumber { get; }
        public string Title { get; }
        public TimeSpan Duration { get; }
        public DateTime ModifyTime { get; }
        
        public async Task<MediaPlaybackItem> GetPlaybackItem()
        {
            var file = await base.GetFileAsync();
            MediaPlaybackItem item = null;

            _playbackItem?.TryGetTarget(out item);

            if (item is null)
            {
                item = new MediaPlaybackItem(MediaSource.CreateFromStorageFile(file));
                _playbackItem = new WeakReference<MediaPlaybackItem>(item);
            }

            return item;
        }

        public int CompareTo(MusicFile other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;

            return SystemStringSorter.Compare(Title, other.Title);
        }

        public int CompareTo(object obj)
        {
            if (ReferenceEquals(this, obj)) return 0;
            if (ReferenceEquals(null, obj)) return 1;
            if (obj.GetType() != typeof(MusicFile)) return 1;

            return CompareTo((MusicFile) obj);
        }
    }
}