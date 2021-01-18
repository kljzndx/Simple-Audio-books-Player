using System;
using System.Linq;
using Windows.Foundation;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using System.ComponentModel;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.ViewModels.Extensions;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.AudioPlayer
{
    public sealed partial class CustomMediaPlayerElement : UserControl
    {
        private MediaPlayer _player;
        private MediaPlaybackItem _currentItem;

        public event TypedEventHandler<CustomMediaPlayerElement, PlayerPositionChangeEventArgs> PositionChanged;
        public event TypedEventHandler<CustomMediaPlayerElement, PlayerNowPlaybackItemChangeEventArgs> NowPlaybackItemChanged;

        private readonly PlayerSettingProperties _settings = PlayerSettingProperties.Current;
        private readonly PlaybackListDataServer _dataServer = PlaybackListDataServer.Current;

        private bool? _isPressPositionControlButton = false;
        private bool _isUserChangePosition;

        public CustomMediaPlayerElement()
        {
            this.InitializeComponent();

            MyTransportControls.CoverButton_Click += (s, e) => CoverButton_Click?.Invoke(s, e);
            _settings.PropertyChanged += Settings_PropertyChanged;
        }

        public event RoutedEventHandler CoverButton_Click;

        private bool TryGetSession(out MediaPlaybackSession session)
        {
            session = _player?.PlaybackSession;
            return session != null;
        }

        private void Play()
        {
            if (!TryGetSession(out var session) || session.PlaybackState != MediaPlaybackState.Playing)
                _player.Play();
        }

        #region Player setup methods

        public void SetMediaPlayer(MediaPlayer mediaPlayer)
        {
            if (_player != null)
            {
                this.LogByObject("正在卸载旧播放器");
                Uninstall(_player);
            }

            this.LogByObject("正在配置播放器");
            _player = mediaPlayer;
            Root_MediaPlayerElement.SetMediaPlayer(mediaPlayer);
            Install(mediaPlayer);
        }

        private void Install(MediaPlayer player)
        {
            player.SourceChanged += Player_SourceChanged;
            player.MediaOpened += Player_MediaOpened;
            player.MediaFailed += Player_MediaFailed;
            player.MediaEnded += Player_MediaEnded;
            player.VolumeChanged += Player_VolumeChanged;

            var session = player.PlaybackSession;
            session.PositionChanged += Player_Session_PositionChanged;
            session.PlaybackRateChanged += Player_Session_PlaybackRateChanged;
        }

        private void Uninstall(MediaPlayer player)
        {
            player.SourceChanged -= Player_SourceChanged;
            player.MediaOpened -= Player_MediaOpened;
            player.MediaFailed -= Player_MediaFailed;
            player.MediaEnded -= Player_MediaEnded;
            player.VolumeChanged -= Player_VolumeChanged;

            var session = player.PlaybackSession;
            session.PositionChanged -= Player_Session_PositionChanged;
            session.PlaybackRateChanged -= Player_Session_PlaybackRateChanged;
        }

        private void InitPlayerSettings()
        {
            var source = _player.Source as MediaPlaybackList;
            // 音量
            {
                _player.Volume = _settings.Volume;
            }

            {
                if (source != null)
                    source.CurrentItemChanged += Source_CurrentItemChanged;
            }
        }

        #endregion
        #region Player position methods

        public void SetPosition(TimeSpan position)
        {
            if (TryGetSession(out var session))
            {
                _isUserChangePosition = true;
                this.LogByObject("设置播放进度，并触发进度更改事件");
                session.Position = position;
                PositionChanged?.Invoke(this, new PlayerPositionChangeEventArgs(true, position));
                _isUserChangePosition = false;
            }
        }

        private async Task PressPositionButton(bool isAdd)
        {
            if (_isPressPositionControlButton != false)
                return;

            _isPressPositionControlButton = null;
            await Task.Delay(TimeSpan.FromSeconds(1));

            if (_isPressPositionControlButton == null)
                _isPressPositionControlButton = true;
            else return;

            _player.Pause();

            string optionInfo = isAdd ? "快进" : "快退";
            while (_isPressPositionControlButton == true)
            {
                if (isAdd)
                    SetPosition(_player.PlaybackSession.Position + TimeSpan.FromSeconds(1));
                else
                    SetPosition(_player.PlaybackSession.Position - TimeSpan.FromSeconds(1));
                this.LogByObject(String.Format("{0} 1秒", optionInfo));

                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }

            this.LogByObject($"完成 {optionInfo} 操作");
        }

        private async Task ReleasePositionButton(bool isNextSong)
        {
            bool? b = _isPressPositionControlButton;
            _isPressPositionControlButton = false;

            if (b == null && _player.Source is MediaPlaybackList mpl)
            {
                string optionInfo = isNextSong ? "下一曲" : "上一曲";
                this.LogByObject($"执行 {optionInfo} 操作");

                if (isNextSong)
                {
                    if (mpl.CurrentItemIndex < mpl.Items.Count - 1)
                        mpl.MoveNext();
                    else
                        await _dataServer.NextClip();
                }
                else
                {
                    if (mpl.CurrentItemIndex > 0)
                        mpl.MovePrevious();
                    else
                        await _dataServer.PreviousClip();
                }

                _settings.CurrentLoopingTimes = 0;
            }
            else
                _player.Play();
        }

        #endregion

        #region Player source events

        private async void Player_SourceChanged(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender.Source is null)
                {
                    this.LogByObject("重置播放源");
                    NowPlaybackItemChanged?.Invoke(this, new PlayerNowPlaybackItemChangeEventArgs(_currentItem, null));
                    _currentItem = null;
                }

                this.LogByObject("初始化播放器");
                InitPlayerSettings();
            });
        }

        private async void Player_MediaOpened(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender.Source is MediaPlaybackItem mpi)
                {
                    NowPlaybackItemChanged?.Invoke(this, new PlayerNowPlaybackItemChangeEventArgs(_currentItem, mpi));
                    _currentItem = mpi;
                }
            });
        }

        private async void Player_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await args.ExtendedErrorCode.ShowErrorDialog();
            });
        }

        private async void Player_MediaEnded(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (_settings.SingleLoopingModeEnable)
                {
                    if (_settings.LoopingTimes >= 1)
                    {
                        if (_settings.CurrentLoopingTimes < _settings.LoopingTimes)
                        {
                            _settings.CurrentLoopingTimes++;
                            Play();
                            return;
                        }
                        else
                            _settings.CurrentLoopingTimes = 0;
                    }
                    else
                    {
                        Play();
                        return;
                    }
                }

                await _dataServer.NextClip();
            });
        }

        private async void Source_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                if (args.Reason == MediaPlaybackItemChangedReason.EndOfStream &&
                    _settings.SingleLoopingModeEnable)
                {
                    if (_settings.LoopingTimes >= 1)
                    {
                        if (_settings.CurrentLoopingTimes < _settings.LoopingTimes)
                        {
                            _settings.CurrentLoopingTimes++;
                            sender.MovePrevious();
                            return;
                        }
                        else
                            _settings.CurrentLoopingTimes = 0;
                    }
                    else
                    {
                        sender.MovePrevious();
                        return;
                    }
                }

                var oldItem = _currentItem;
                _currentItem = args.NewItem;

                if (TryGetSession(out var session))
                    session.PlaybackRate = _settings.PlaybackRate;

                if (args.NewItem != null && args.Reason == MediaPlaybackItemChangedReason.AppRequested)
                {
                    var fileDto = _dataServer.Data.First();
                    MyTransportControls.CoverSource = await fileDto.Group.GetCover();
                }

                await _dataServer.PlaybackList_CurrentItemChanged(sender, args);
                NowPlaybackItemChanged?.Invoke(this, new PlayerNowPlaybackItemChangeEventArgs(oldItem, args.NewItem));
            });
        }

        #endregion
        #region Player properties events

        private async void Player_Session_PositionChanged(MediaPlaybackSession sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (!_isUserChangePosition)
                    PositionChanged?.Invoke(this, new PlayerPositionChangeEventArgs(false, sender.Position));
            });
        }

        private void Player_VolumeChanged(MediaPlayer sender, object args)
        {
            _settings.Volume = sender.Volume;
        }

        private async void Player_Session_PlaybackRateChanged(MediaPlaybackSession sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MyTransportControls.PlaybackRate = sender.PlaybackRate;
            });
        }

        private async void MyTransportControls_OnRateValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (e.NewValue != _settings.PlaybackRate)
                    _settings.PlaybackRate = e.NewValue;
            });
        }

        private void MyTransportControls_OnSingleLoopingToggled(object sender, RoutedEventArgs e)
        {
            var the = (ToggleSwitch)sender;
            _settings.SingleLoopingModeEnable = the.IsOn;
        }

        private void MyTransportControls_OnLoopingTimesValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            _settings.LoopingTimes = e.NewValue;
        }

        #endregion
        #region Player position controller events

        private void MyTransportControls_OnPositionSlider_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            _isUserChangePosition = true;
        }

        private void MyTransportControls_OnPositionSlider_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            if (TryGetSession(out var session))
                PositionChanged?.Invoke(this, new PlayerPositionChangeEventArgs(true, session.Position));

            _isUserChangePosition = false;
        }

        private async void MyTransportControls_OnRewindButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await PressPositionButton(false);
        }

        private async void MyTransportControls_OnRewindButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            await ReleasePositionButton(false);
        }

        private async void MyTransportControls_OnFastForwardButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await PressPositionButton(true);
        }

        private async void MyTransportControls_OnFastForwardButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            await ReleasePositionButton(true);
        }

        #endregion

        private async void Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_settings.PlaybackRate):
                        if (TryGetSession(out var session))
                            session.PlaybackRate = _settings.PlaybackRate;
                        break;
                }
            });
        }
    }
}
