using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class PlaybackListDataServer : IDataServer<MusicFileDTO, MusicFileDTO>
    {
        public static PlaybackListDataServer Current = new PlaybackListDataServer();

        private int _tempItemId = -1;
        private FileGroupDTO _currentGroup;
        private MusicListSortMembers _currentSortMethod;

        private readonly MusicListSettingProperties _musicListSettings = MusicListSettingProperties.Current;
        private readonly List<List<MusicFileDTO>> _tempList = new List<List<MusicFileDTO>>();
        private readonly MediaPlaybackList _playbackList = new MediaPlaybackList();
        private MediaPlayer _player;

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
            _player = App.MediaPlayer;
            _player.Source = _playbackList;

            var record = _recordServer.Data.OrderBy(r => r.PlayDate).LastOrDefault();
            if (record != null)
                await SetSource(record);

            DataLoaded?.Invoke(this, Data.ToList());

            _musicServer.DataRemoved += MusicServer_DataRemoved;
            _player.MediaEnded += Player_MediaEnded;
            _playbackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
        }

        public async Task SetSource(PlaybackRecordDTO record)
        {
            bool hasData = Data.Any();

            InitData(record.Group, record.SortMethod, record.IsReverse);

            await PlayTo(record.TrackId);

            if (hasData)
                BeginToPlay();
        }

        public async Task SetSource(MusicFileDTO playTo)
        {
            InitData(playTo.Group, _musicListSettings.SortMethod);

            uint trackId = (uint) Data.IndexOf(playTo);
            await PlayTo(trackId);

            BeginToPlay();
        }

        public async Task SetSource(FileGroupDTO groupDto)
        {
            InitData(groupDto, _musicListSettings.SortMethod);

            await PlayTo(0);

            BeginToPlay();
        }

        private void InitData(FileGroupDTO groupDto, MusicListSortMembers method, bool? isReverse = null)
        {
            bool hasData = Data.Any();

            if (!groupDto.Equals(_currentGroup) || _currentSortMethod != method)
            {
                _currentGroup = groupDto;

                var sortSelector = MusicSortDeserialization.Deserialize(method);
                IEnumerable<MusicFileDTO> source = _musicServer.Data.Where(m => m.Group.Equals(groupDto)).OrderBy(sortSelector.Invoke);

                if (isReverse ?? _musicListSettings.IsReverse)
                    source = source.Reverse();

                var list = source.ToList();
                SplitList(list);

                if (hasData)
                    DataRemoved?.Invoke(this, Data.ToList());

                _playbackList.Items.Clear();
                Data.Clear();
                foreach (var musicFile in list)
                    Data.Add(musicFile);

                if (hasData)
                    DataAdded?.Invoke(this, Data.ToList());

                _currentSortMethod = method;
                _tempItemId = -1;
            }
        }

        private void SplitList(IEnumerable<MusicFileDTO> files)
        {
            _tempList.Clear();
            var queue = new Queue<MusicFileDTO>(files);

            while (queue.Any())
            {
                var temp = new List<MusicFileDTO>();

                for (int i = 0; i < 10; i++)
                    if (queue.Any())
                        temp.Add(queue.Dequeue());
                    else
                        break;

                _tempList.Add(temp);
            }
        }
        
        private async Task PlayTo(uint trackId)
        {
            var groupId = (int) Math.Ceiling((trackId + 1) / 10D) - 1;
            
            var group = _tempList[groupId];
            var mfId = (uint) (trackId < 10 ? trackId : trackId - (10 * groupId));

            if (_tempItemId != groupId)
            {
                _tempItemId = groupId;
                _playbackList.Items.Clear();

                foreach (var fileDto in group)
                    _playbackList.Items.Add(await fileDto.GetPlaybackItem());
            }

            _playbackList.MoveTo(mfId);
        }

        private void BeginToPlay()
        {
            if (_player.PlaybackSession != null &&
                _player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                _player.Play();
            }
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

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var id = 10 * (_tempItemId + 1);
                if (id < Data.Count)
                    await SetSource(Data[id]);
            });
        }

        private async void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            var cw = CoreApplication.MainView.CoreWindow;
            await cw.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                int id = (int) (_tempItemId * 10 + sender.CurrentItemIndex);
                if (id == -1)
                    return;

                var mfd = Data[id];
                await _recordServer.SetRecord(new PlaybackRecordDTO(mfd.Title, mfd.Group, (uint) id, _currentSortMethod,
                    _musicListSettings.IsReverse));
            });
        }
    }
}