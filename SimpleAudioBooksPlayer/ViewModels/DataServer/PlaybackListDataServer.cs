using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
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

        private FileGroupDTO _currentGroup;
        private PlaybackRecordDTO _currentRecordDto;
        private MusicListSortMembers _currentSortMethod;
        private bool _isPreLoadClip;

        private uint _playingId;
         
        private int _clipId = -1;
        private readonly List<List<MusicFileDTO>> _clipList = new List<List<MusicFileDTO>>();

        private readonly MusicListSettingProperties _musicListSettings = MusicListSettingProperties.Current;
        private readonly MediaPlaybackList _playbackList = new MediaPlaybackList();
        private MediaPlayer _player;

        private readonly MusicFileDataServer _musicServer = MusicFileDataServer.Current;
        private readonly PlaybackRecordDataServer _recordServer = PlaybackRecordDataServer.Current;

        private PlaybackListDataServer()
        {
            Data = new ObservableCollection<MusicFileDTO>();

            App.Current.Suspending += App_Suspending;
            App.Current.EnteredBackground += App_EnteredBackground;
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
            _playbackList.CurrentItemChanged += PlaybackList_CurrentItemChanged;
        }

        public async Task PreviousClip()
        {
            if (!Data.Any())
                return;

            _isPreLoadClip = false;

            if (_playingId == 0)
                await PlayTo((uint) Data.IndexOf(Data.Last()));
            else
                await PlayTo(_playingId - 1);
        }

        public async Task NextClip()
        {
            if (!Data.Any())
                return;

            _isPreLoadClip = false;

            if (_playingId == Data.Count - 1)
                await PlayTo((uint) Data.IndexOf(Data.First()));
            else
                await PlayTo(_playingId + 1);
        }

        #region Source setter
        
        public async Task SetSource(PlaybackRecordDTO record)
        {
            bool hasData = Data.Any();

            await SetPlayedTime();
            
            InitData(record.Group, record.SortMethod, record.IsReverse);

            await PlayTo(record.TrackId);

            if (_player.PlaybackSession != null)
                _player.PlaybackSession.Position = record.PlayedTime;

            if (hasData)
                BeginToPlay();
        }

        public async Task SetSource(MusicFileDTO playTo)
        {
            await SetPlayedTime();

            InitData(playTo.Group, _musicListSettings.SortMethod);

            uint trackId = (uint) Data.IndexOf(playTo);
            await PlayTo(trackId);

            BeginToPlay();
        }

        public async Task SetSource(FileGroupDTO groupDto)
        {
            await SetPlayedTime();

            InitData(groupDto, _musicListSettings.SortMethod);

            await PlayTo(0);

            BeginToPlay();
        }

        #endregion

        private async Task SetPlayedTime()
        {
            if (_player.PlaybackSession != null && _currentRecordDto != null)
            {
                _currentRecordDto.PlayedTime = _player.PlaybackSession.Position;
                await _recordServer.SetRecord(_currentRecordDto);
            }
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
                _clipId = -1;
            }
        }

        private void SplitList(IEnumerable<MusicFileDTO> files)
        {
            _clipList.Clear();
            var queue = new Queue<MusicFileDTO>(files);

            while (queue.Any())
            {
                var temp = new List<MusicFileDTO>();

                for (int i = 0; i < 10; i++)
                    if (queue.Any())
                        temp.Add(queue.Dequeue());
                    else
                        break;

                _clipList.Add(temp);
            }
        }
        
        private async Task PlayTo(uint trackId)
        {
            _playingId = trackId;
            var clipId = (int) Math.Ceiling((trackId + 1) / 10D) - 1;
            
            var clip = _clipList[clipId];
            var mfId = (uint) (trackId < 10 ? trackId : trackId - (10 * clipId));

            if (_clipId != clipId)
            {
                _clipId = clipId;
                _playbackList.Items.Clear();

                foreach (var fileDto in clip)
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

        private async void PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            var cw = CoreApplication.MainView.CoreWindow;
            await cw.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (args.Reason == MediaPlaybackItemChangedReason.EndOfStream
                    && !_isPreLoadClip && sender.Items.Count != 1 && sender.CurrentItemIndex == sender.Items.Count - 1)
                {
                    var cid = _clipId < _clipList.Count - 1 ? _clipId + 1 : 0;
                    foreach (var fileDto in _clipList[cid])
                        sender.Items.Add(await fileDto.GetPlaybackItem());

                    _isPreLoadClip = true;
                }

                if (_isPreLoadClip && sender.CurrentItemIndex == 10)
                {
                    _isPreLoadClip = false;
                    _clipId = _clipId < _clipList.Count - 1 ? _clipId + 1 : 0;
                    for (int i = 0; i < 10; i++)
                        sender.Items.RemoveAt(0);
                }

                int id = (int) (_clipId * 10 + sender.CurrentItemIndex);
                if (id < 0)
                    return;

                _playingId = (uint) id;

                var mfd = Data[id];
                _currentRecordDto = new PlaybackRecordDTO(mfd.Title, mfd.Group, (uint) id, _currentSortMethod, _musicListSettings.IsReverse);
                await _recordServer.SetRecord(_currentRecordDto);
            });
        }

        private async void App_Suspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SetPlayedTime();
            deferral.Complete();
        }

        private async void App_EnteredBackground(object sender, EnteredBackgroundEventArgs e)
        {
            var deferral = e.GetDeferral();
            await SetPlayedTime();
            deferral.Complete();
        }
    }
}