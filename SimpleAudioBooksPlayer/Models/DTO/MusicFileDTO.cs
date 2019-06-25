using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class MusicFileDTO : ObservableObject
    {
        private WeakReference<StorageFile> _file;
        private MediaPlaybackItem _playbackItem;

        private bool _isPlaying;

        public MusicFileDTO(MusicFile source)
        {
            Group = FileGroupDataServer.Current.Data.First(g => g.Index == source.GroupId);
            TrackNumber = source.TrackNumber;
            Title = source.Title;
            Duration = source.Duration;
            FileName = source.FileName;
            FilePath = source.FilePath;
            ModifyTime = source.ModifyTime;
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        public FileGroupDTO Group { get; }

        public uint TrackNumber { get; set; }
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
    }
}