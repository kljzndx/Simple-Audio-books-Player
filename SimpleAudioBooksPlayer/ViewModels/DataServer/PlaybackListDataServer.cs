using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using SimpleAudioBooksPlayer.Models;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class PlaybackListDataServer : IDataServer<MusicFileDTO, MusicFileDTO>
    {
        public static PlaybackListDataServer Current = new PlaybackListDataServer();

        private int _tempItemId = -1;
        private FileGroupDTO _currentGroup;
        private MusicListSortMembers _currentSortMethod;

        private readonly List<List<MusicFileDTO>> _tempList = new List<List<MusicFileDTO>>();
        private readonly MediaPlaybackList _playbackList = new MediaPlaybackList();
        private readonly MusicListSettingProperties _musicListSettings = MusicListSettingProperties.Current;

        private readonly MusicFileDataServer _musicServer = MusicFileDataServer.Current;
        private readonly PlaybackRecordDataServer _recordServer = PlaybackRecordDataServer.Current;

        private PlaybackListDataServer()
        {
            Data = new ObservableCollection<MusicFileDTO>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<MusicFileDTO> Data { get; }

        public event EventHandler<IEnumerable<MusicFileDTO>> DataLoaded;
        public event EventHandler<IEnumerable<MusicFileDTO>> DataAdded;
        public event EventHandler<IEnumerable<MusicFileDTO>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;

            var record = _recordServer.Data.OrderBy(r => r.PlayDate).LastOrDefault();
            if (record != null)
                await SetSource(record);

            DataLoaded?.Invoke(this, Data.ToList());

            _playbackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
            _musicServer.DataRemoved += MusicServer_DataRemoved;
        }

        public async Task SetSource(PlaybackRecordDTO record)
        {
            if (!_currentGroup.Equals(record.Group) || _currentSortMethod != record.SortMethod)
            {
                _currentGroup = record.Group;
                var sortSelector = MusicSortDeserialization.Deserialize(record.SortMethod);

                IEnumerable<MusicFileDTO> source = _musicServer.Data.Where(m => record.Group.Equals(m.Group))
                    .OrderBy(sortSelector.Invoke);
                if (record.IsReverse)
                    source = source.Reverse();
                var list = source.ToList();
                SplitList(list);

                bool hasData = Data.Any();
                if (hasData)
                    DataRemoved?.Invoke(this, Data.ToList());

                Data.Clear();
                foreach (var musicFileDto in list)
                    Data.Add(musicFileDto);

                if (hasData)
                    DataAdded?.Invoke(this, Data.ToList());

                _currentSortMethod = record.SortMethod;
            }

            await PlayTo(record.TrackId);
        }

        public async Task SetSource(MusicFileDTO playTo, MusicListSortMembers sortMethod)
        {
            if (_currentGroup.Equals(playTo.Group) || _currentSortMethod != sortMethod)
            {
                _currentGroup = playTo.Group;

                var sortSelector = MusicSortDeserialization.Deserialize(sortMethod);
                IEnumerable<MusicFileDTO> source = _musicServer.Data.Where(m => m.Group.Equals(playTo.Group))
                    .OrderBy(sortSelector.Invoke);

                if (_musicListSettings.IsReverse)
                    source = source.Reverse();

                var list = source.ToList();
                SplitList(list);

                DataRemoved?.Invoke(this, Data.ToList());

                Data.Clear();
                foreach (var musicFile in list)
                    Data.Add(musicFile);

                DataAdded?.Invoke(this, Data.ToList());

                _currentSortMethod = sortMethod;
            }

            uint trackId = (uint) Data.IndexOf(playTo);
            await _recordServer.SetRecord(new PlaybackRecordDTO(playTo.Title, playTo.Group, trackId, sortMethod, _musicListSettings.IsReverse));
            await PlayTo(trackId);
        }

        private void SplitList(IEnumerable<MusicFileDTO> files)
        {
            _tempList.Clear();
            var queue = new Queue<MusicFileDTO>(files);

            while (queue.Any())
            {
                var temp = new List<MusicFileDTO>();

                for (int i = 0; i < 10; i++)
                    temp.Add(queue.Dequeue());

                _tempList.Add(temp);
            }
        }

        private async Task PlayTo(uint trackId)
        {
            var groupId = (int)(((trackId + 1) / 10) - 1);
            var group = _tempList[groupId];
            var mfId = (uint)(trackId < 10 ? trackId : trackId - (10 * (groupId + 1)));

            if (_tempItemId != groupId)
            {
                _playbackList.Items.Clear();
                foreach (var fileDto in group)
                    _playbackList.Items.Add(await fileDto.GetPlaybackItem());
                _tempItemId = groupId;
            }

            _playbackList.MoveTo(mfId);
        }

        private async void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                int id = (int)(_tempItemId * 10 + sender.CurrentItemIndex);
                var mfd = Data[id];
                await _recordServer.SetRecord(new PlaybackRecordDTO(mfd.Title, mfd.Group, (uint) id, _currentSortMethod,
                    _musicListSettings.IsReverse));
            });
        }

        private async void MusicServer_DataRemoved(object sender, IEnumerable<MusicFileDTO> e)
        {
            foreach (var fileDto in Data.Where(src => e.Any(f => f.FilePath == src.FilePath)))
            {
                var pb = await fileDto.GetPlaybackItem();
                if (_playbackList.Items.Contains(pb))
                    _playbackList.Items.Remove(pb);

                Data.Remove(fileDto);
            }
        }
    }
}