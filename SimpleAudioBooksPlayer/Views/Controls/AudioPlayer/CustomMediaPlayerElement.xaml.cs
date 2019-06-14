using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
using System.ComponentModel;
using System.Threading.Tasks;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.AudioPlayer
{
    public sealed partial class CustomMediaPlayerElement : UserControl
    {
        private static MediaPlayer player;
        public static MediaPlaybackItem CurrentItem { get; private set; }

        public static event TypedEventHandler<CustomMediaPlayerElement, PlayerPositionChangeEventArgs> PositionChanged;
        public static event TypedEventHandler<CustomMediaPlayerElement, PlayerNowPlaybackItemChangeEventArgs> NowPlaybackItemChanged;

        private readonly PlayerSettingProperties _settings = PlayerSettingProperties.Current;

        private bool? _isPressPositionControlButton = false;
        private bool _isUserChangePosition;

        public CustomMediaPlayerElement()
        {
            this.InitializeComponent();
            _settings.PropertyChanged += Settings_PropertyChanged;
            MyTransportControls.RepeatMode_SelectedID = (int) _settings.RepeatMode;

            MyTransportControls.CoverButton_Click += (s, e) => CoverButton_Click?.Invoke(s, e);
        }

        public event RoutedEventHandler CoverButton_Click;

        private static bool TryGetSession(out MediaPlaybackSession session)
        {
            session = player?.PlaybackSession;
            return session != null;
        }

        #region Player setup methods

        public void SetMediaPlayer(MediaPlayer mediaPlayer)
        {
            if (player != null)
                Uninstall(player);

            player = mediaPlayer;
            Root_MediaPlayerElement.SetMediaPlayer(mediaPlayer);
            Install(mediaPlayer);
        }

        private void Install(MediaPlayer player)
        {
            player.SourceChanged += Player_SourceChanged;
            player.MediaOpened += Player_MediaOpened;
            player.MediaFailed += Player_MediaFailed;
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
            player.VolumeChanged -= Player_VolumeChanged;

            var session = player.PlaybackSession;
            session.PositionChanged -= Player_Session_PositionChanged;
            session.PlaybackRateChanged -= Player_Session_PlaybackRateChanged;
        }

        private void InitPlayerSettings()
        {
            var source = player.Source as MediaPlaybackList;
            // 循环模式
            {
                if (source != null)
                {
                    source.AutoRepeatEnabled = true;

                    switch ((int)_settings.RepeatMode)
                    {
                        case 0:
                            player.IsLoopingEnabled = true;
                            break;
                        case 1:
                            source.ShuffleEnabled = false;
                            break;
                        case 2:
                            source.ShuffleEnabled = true;
                            break;
                    }
                }
            }
            // 音量
            {
                player.Volume = _settings.Volume;
            }

            {
                if (source != null)
                    source.CurrentItemChanged += Source_CurrentItemChanged;
            }
        }

        #endregion
        #region Player position methods

        private void SetPosition(TimeSpan position)
        {
            if (TryGetSession(out var session))
            {
                _isUserChangePosition = true;
                session.Position = position;
                PositionChanged?.Invoke(this, new PlayerPositionChangeEventArgs(true, position));
                _isUserChangePosition = false;
            }
        }

        public static void SetPosition_Global(TimeSpan position)
        {
            if (TryGetSession(out var session))
            {
                session.Position = position;
                PositionChanged?.Invoke(null, new PlayerPositionChangeEventArgs(true, position));
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

            player.Pause();

            //string optionInfo = isAdd ? "快进" : "快退";
            //this.LogByObject($"正在执行 {optionInfo} 操作");

            while (_isPressPositionControlButton == true)
            {
                if (isAdd)
                    SetPosition(player.PlaybackSession.Position + TimeSpan.FromSeconds(1));
                else
                    SetPosition(player.PlaybackSession.Position - TimeSpan.FromSeconds(1));
                //this.LogByObject($"已 {optionInfo} 1秒");

                await Task.Delay(TimeSpan.FromMilliseconds(200));
            }

            //this.LogByObject($"已完成 {optionInfo} 操作");
        }

        private void ReleasePositionButton(bool isNextSong)
        {
            bool? b = _isPressPositionControlButton;
            _isPressPositionControlButton = false;

            if (b == null && player.Source is MediaPlaybackList mpl)
            {
                //string optionInfo = isNextSong ? "下一曲" : "上一曲";
                //this.LogByObject($"正在执行 {optionInfo} 操作");

                if (isNextSong)
                    mpl.MoveNext();
                else
                    mpl.MovePrevious();

                //this.LogByObject($"完成 {optionInfo} 操作");
            }
            else
                player.Play();
        }

        #endregion

        #region Player source events


        private async void Player_SourceChanged(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender.Source is null)
                {
                    NowPlaybackItemChanged?.Invoke(this, new PlayerNowPlaybackItemChangeEventArgs(CurrentItem, null));
                    CurrentItem = null;
                }

                InitPlayerSettings();
            });
        }

        private async void Player_MediaOpened(MediaPlayer sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (sender.Source is MediaPlaybackItem mpi)
                {
                    NowPlaybackItemChanged?.Invoke(this, new PlayerNowPlaybackItemChangeEventArgs(CurrentItem, mpi));
                    CurrentItem = mpi;
                }
            });
        }

        private void Player_MediaFailed(MediaPlayer sender, MediaPlayerFailedEventArgs args)
        {

        }

        private async void Source_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                NowPlaybackItemChanged?.Invoke(this, new PlayerNowPlaybackItemChangeEventArgs(CurrentItem, args.NewItem));
                CurrentItem = args.NewItem;

                if (TryGetSession(out var session))
                    session.PlaybackRate = _settings.PlaybackRate;
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

        private void MyTransportControls_OnRewindButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ReleasePositionButton(false);
        }

        private async void MyTransportControls_OnFastForwardButton_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            await PressPositionButton(true);
        }

        private void MyTransportControls_OnFastForwardButton_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            ReleasePositionButton(true);
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

        private void MyTransportControls_OnRepeatModeSelectionChanged(CustomTransportControls sender, KeyValuePair<int, string> args)
        {
            _settings.RepeatMode = (PlaybackRepeatModeEnum) args.Key;

            if (player.Source is MediaPlaybackList mpl)
            {
                switch (args.Key)
                {
                    case 0:
                        player.IsLoopingEnabled = true;
                        break;
                    case 1:
                        mpl.ShuffleEnabled = false;
                        break;
                    case 2:
                        mpl.ShuffleEnabled = true;
                        break;
                }
            }
        }
    }
}
