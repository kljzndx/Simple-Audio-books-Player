using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Media.Playback;
using Windows.UI.Core;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileFactories;
using SimpleAudioBooksPlayer.Models.FileModels;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class PlaybackListDataServer : IDataServer<MusicFile, MusicFile>
    {
        public static PlaybackListDataServer Current = new PlaybackListDataServer();

        private PlaybackRecordDTO _currentRecordDto;
        private MusicListSortMembers _currentSortMethod;
        private bool _isPreLoadClip;

        private uint _playingId;
         
        private int _clipId = -1;
        private readonly List<List<MusicFile>> _clipList = new List<List<MusicFile>>();

        private readonly MusicListSettingProperties _musicListSettings = MusicListSettingProperties.Current;
        private readonly MediaPlaybackList _playbackList = new MediaPlaybackList();
        private MediaPlayer _player;

        private readonly MusicFileDataServer _musicServer = MusicFileDataServer.Current;
        private readonly PlaybackRecordDataServer _recordServer = PlaybackRecordDataServer.Current;

        private PlaybackListDataServer()
        {
            Data = new ObservableCollection<MusicFile>();

            Data.CollectionChanged += Data_CollectionChanged;
            App.Current.Suspending += App_Suspending;
            App.Current.EnteredBackground += App_EnteredBackground;
        }

        public bool IsInit { get; private set; }

        public ObservableCollection<MusicFile> Data { get; }
        
        public MusicFile CurrentMusic { get; private set; }
        public FileGroupDTO CurrentGroup { get; private set; }

        public event EventHandler<IEnumerable<MusicFile>> DataLoaded;
        public event EventHandler<IEnumerable<MusicFile>> DataAdded;
        public event EventHandler<IEnumerable<MusicFile>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;
            _player = App.MediaPlayer;

            this.LogByObject("初始化播放列表服务器");
            var record = _recordServer.Data.OrderBy(r => r.PlayDate).LastOrDefault();
            if (record != null)
                await SetSource(record);

            DataLoaded?.Invoke(this, Data.ToList());
        }

        public async Task PreviousClip()
        {
            if (!Data.Any())
                return;

            this.LogByObject("上一组播放项");
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

            this.LogByObject("下一组播放项");
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

            await InitData(record.Group, record.SortMethod, record.IsReverse);

            await PlayTo(record.TrackId);

            if (_player.PlaybackSession != null)
                _player.PlaybackSession.Position = record.PlayedTime;

            if (hasData)
                BeginToPlay();
        }

        public async Task SetSource(MusicFile playTo, bool isRefresh = false)
        {
            // 防止重新扫描文件后无法播放新的文件列表的问题
            if (playTo.Group.Equals(CurrentGroup) && isRefresh)
                CurrentGroup = null;

            await SetPlayedTime();

            await InitData(playTo.Group, _musicListSettings.SortMethod);

            uint trackId = (uint) Data.IndexOf(playTo);
            await PlayTo(trackId);

            BeginToPlay();
        }

        public async Task SetSource(FileGroupDTO groupDto)
        {
            await SetPlayedTime();

            await InitData(groupDto, _musicListSettings.SortMethod);

            await PlayTo(0);

            BeginToPlay();
        }

        #endregion

        private async Task SetPlayedTime()
        {
            if (_player?.PlaybackSession != null && _currentRecordDto != null)
            {
                this.LogByObject("设置上次播放时间");
                _currentRecordDto.PlayedTime = _player.PlaybackSession.Position;
                await _recordServer.SetRecord(_currentRecordDto);
            }
        }

        private async Task InitData(FileGroupDTO groupDto, MusicListSortMembers method, bool? isReverse = null)
        {
            bool hasData = Data.Any();
            bool isGroupEquals = groupDto.Equals(CurrentGroup);

            if (!isGroupEquals || _currentSortMethod != method)
            {
                this.LogByObject("筛选数据源并排序数据");

                var data = new List<MusicFile>();
                
                if (!isGroupEquals)
                {
                    if (_musicServer.CurrentGroup != null && groupDto.Equals(_musicServer.CurrentGroup))
                        data = _musicServer.Data.ToList();
                    else
                        data = await FileDataScanner.ScanMusicData(groupDto);
                    
                    await SubtitleFileDataServer.Current.Scan(groupDto);
                }
                
                var sortSelector = MusicSortDeserialization.Deserialize(method);
                IEnumerable<MusicFile> source = data.OrderBy(sortSelector.Invoke);

                CurrentGroup = groupDto;

                if (isReverse ?? _musicListSettings.IsReverse)
                    source = source.Reverse();

                var list = source.ToList();
                SplitList(list);

                if (hasData)
                {
                    foreach (var fileDto in Data.Where(m => m.IsPlaying))
                        fileDto.IsPlaying = false;
                    
                    DataRemoved?.Invoke(this, Data.ToList());
                }

                this.LogByObject("添加数据");
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

        private void SplitList(IEnumerable<MusicFile> files)
        {
            this.LogByObject("分割数据源");
            _clipList.Clear();
            var queue = new Queue<MusicFile>(files);

            while (queue.Any())
            {
                var temp = new List<MusicFile>();

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
            this.LogByObject("计算各项数据");
            _playingId = trackId;
            var clipId = (int) Math.Ceiling((trackId + 1) / 10D) - 1;
            
            var clip = _clipList[clipId];
            var mfId = (uint) (trackId < 10 ? trackId : trackId - (10 * clipId));

            this.LogByObject($"计算结果： trackId = {trackId} clipId = {clipId} mfId = {mfId}");

            if (_clipId != clipId)
            {
                this.LogByObject("加载播放项");
                _clipId = clipId;
                _playbackList.Items.Clear();

                foreach (var fileDto in clip)
                    _playbackList.Items.Add(await fileDto.GetPlaybackItem());
            }

            this.LogByObject("移动播放头");
            _playbackList.MoveTo(mfId);
        }

        public async Task PlayTo(int trackId)
        {
            this.LogByObject("外部版 PlayTo 方法被调用");
            await PlayTo((uint) trackId);
            BeginToPlay();
        }

        private void BeginToPlay()
        {
            PlayerSettingProperties.Current.CurrentLoopingTimes = 0;

            if (_player.PlaybackSession != null &&
                _player.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                this.LogByObject("开始播放");
                _player.Play();
            }
        }

        private void Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (Data.Any() && _player.Source != _playbackList)
            {
                this.LogByObject("设置播放源");
                _player.Source = _playbackList;
                Data.CollectionChanged -= Data_CollectionChanged;
            }
        }

        public async Task PlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            var cw = CoreApplication.MainView.CoreWindow;
            await cw.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                var itemCount = sender.Items.Count;
                var currentId = args.NewItem != null ? sender.Items.IndexOf(args.NewItem) : 0;

                if (_isPreLoadClip && currentId >= 10)
                {
                    this.LogByObject("开始清理播放项");
                    _clipId = _clipId < _clipList.Count - 1 ? _clipId + 1 : 0;
                    for (int i = 0; i < 10; i++)
                        sender.Items.RemoveAt(0);

                    _isPreLoadClip = false;
                    itemCount = sender.Items.Count;
                    currentId = args.NewItem != null ? sender.Items.IndexOf(args.NewItem) : 0;
                }

                if (!_isPreLoadClip && itemCount == 10 && currentId == 9)
                {
                    this.LogByObject("开始预加载播放项数据");
                    var cid = _clipId < _clipList.Count - 1 ? _clipId + 1 : 0;
                    foreach (var fileDto in _clipList[cid])
                        sender.Items.Add(await fileDto.GetPlaybackItem());

                    _isPreLoadClip = true;
                    itemCount = sender.Items.Count;
                }

                int id = _clipId * 10 + currentId;
                if (id < 0)
                    return;

                this.LogByObject($"播放轨道信息： PlayId: {id}, ItemCount: {itemCount}, CurrentIndex: {currentId}, ClipId: {_clipId}, PreLoadClip: {_isPreLoadClip}, ChangeReason: {args.Reason.ToString()}");

                _playingId = (uint) id;

                this.LogByObject("更新音频信息");
                var mfd = Data[id];
                if (CurrentMusic != null)
                    CurrentMusic.IsPlaying = false;
                
                CurrentMusic = mfd;
                CurrentMusic.IsPlaying = true;
                PlayerNotifier.RaiseItem(mfd);
                
                this.LogByObject("创建播放记录");
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