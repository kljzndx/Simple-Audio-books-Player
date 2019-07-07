using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.Models.DTO
{
    public class MusicFileDTO : ObservableObject
    {
        private static readonly Regex NumberRegex = new Regex(@"[0-9]+");

        private WeakReference<StorageFile> _file;
        private MediaPlaybackItem _playbackItem;

        private bool _isPlaying;

        private uint _trackNumber;
        private string _title;
        private TimeSpan _duration;
        private DateTime _modifyTime;

        public MusicFileDTO(MusicFile source)
        {
            Group = FileGroupDataServer.Current.Data.First(g => g.Index == source.GroupId);
            _trackNumber = source.TrackNumber;
            _title = source.Title;
            _duration = source.Duration;
            FileName = source.FileName;
            FilePath = source.FilePath;
            _modifyTime = source.ModifyTime;
        }

        public bool IsPlaying
        {
            get => _isPlaying;
            set => Set(ref _isPlaying, value);
        }

        public bool HasRead { get; private set; }

        public FileGroupDTO Group { get; }
        public uint TrackNumber
        {
            get => _trackNumber;
            private set => Set(ref _trackNumber, value);
        }
        public string Title
        {
            get => _title;
            private set => Set(ref _title, value);
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

            TrackNumber = source.TrackNumber;
            Title = source.Title;
            Duration = source.Duration;
            ModifyTime = source.ModifyTime;
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
    }
}