using System;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Newtonsoft.Json;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.Sorters;

namespace SimpleAudioBooksPlayer.Models.FileModels
{
    [JsonObject(MemberSerialization.OptIn)]
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

        [JsonConstructor]
        public MusicFile(string fileName, DateTime modifyTime, uint trackNumber, string title, TimeSpan duration) : this(null, fileName, modifyTime, trackNumber, title, duration)
        {
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        [JsonProperty]
        public uint TrackNumber { get; }
        [JsonProperty]
        public string Title { get; }
        [JsonProperty]
        public TimeSpan Duration { get; }
        [JsonProperty]
        public DateTime ModifyTime { get; }

        public async Task<MediaPlaybackItem> GetPlaybackItem()
        {
            MediaPlaybackItem item = null;

            _playbackItem?.TryGetTarget(out item);

            if (item is null)
            {
                var file = await base.GetFileAsync();
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