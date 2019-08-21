using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

// The Templated Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234235

namespace SimpleAudioBooksPlayer.Views.Controls.AudioPlayer
{
    public sealed class CustomTransportControls : MediaTransportControls
    {
        private Slider _playbackRateSlider;
        private Slider _loopingTimesSlider;

        public CustomTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomTransportControls);
        }

        public static readonly DependencyProperty CoverSourceProperty = DependencyProperty.Register(
            nameof(CoverSource), typeof(ImageSource), typeof(CustomTransportControls), new PropertyMetadata(null));

        public ImageSource CoverSource
        {
            get => (ImageSource)GetValue(CoverSourceProperty);
            set => SetValue(CoverSourceProperty, value);
        }

        public double PlaybackRate
        {
            get => _playbackRateSlider.Value;
            set => _playbackRateSlider.Value = value;
        }

        public event PointerEventHandler PositionSlider_PointerPressed;
        public event PointerEventHandler PositionSlider_PointerReleased;

        public event PointerEventHandler RewindButton_PointerPressed;
        public event PointerEventHandler RewindButton_PointerReleased;

        public event PointerEventHandler FastForwardButton_PointerPressed;
        public event PointerEventHandler FastForwardButton_PointerReleased;

        public event RoutedEventHandler CoverButton_Click;
        public event RangeBaseValueChangedEventHandler RateValueChanged;

        public event RoutedEventHandler SingleLoopingToggled;
        public event RangeBaseValueChangedEventHandler LoopingTimesValueChanged;

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var setting = PlayerSettingProperties.Current;

            if (!OtherSettingProperties.Current.IsPaid)
                OtherSettingProperties.Current.PropertyChanged += OtherSetting_PropertyChanged;

            this.LogByObject("监听进度条的指针事件");
            {
                var progressSlider = (Slider)GetTemplateChild("ProgressSlider");
                progressSlider.AddHandler(PointerPressedEvent, new PointerEventHandler((s, e) => PositionSlider_PointerPressed?.Invoke(s, e)), true);
                progressSlider.AddHandler(PointerReleasedEvent, new PointerEventHandler((s, e) => PositionSlider_PointerReleased?.Invoke(s, e)), true);
                progressSlider.AddHandler(PointerCanceledEvent, new PointerEventHandler((s, e) => PositionSlider_PointerReleased?.Invoke(s, e)), true);
                progressSlider.AddHandler(PointerCaptureLostEvent, new PointerEventHandler((s, e) => PositionSlider_PointerReleased?.Invoke(s, e)), true);

            }

            this.LogByObject("监听快退按钮的指针事件");
            {
                var rewindButton = (AppBarButton)GetTemplateChild("Rewind_Button");
                rewindButton.AddHandler(PointerPressedEvent, new PointerEventHandler((s, e) => RewindButton_PointerPressed?.Invoke(s, e)), true);
                rewindButton.AddHandler(PointerReleasedEvent, new PointerEventHandler((s, e) => RewindButton_PointerReleased?.Invoke(s, e)), true);
                rewindButton.AddHandler(PointerCanceledEvent, new PointerEventHandler((s, e) => RewindButton_PointerReleased?.Invoke(s, e)), true);
                rewindButton.AddHandler(PointerCaptureLostEvent, new PointerEventHandler((s, e) => RewindButton_PointerReleased?.Invoke(s, e)), true);
            }

            this.LogByObject("监听快进按钮的指针事件");
            {
                var fastForwardButton = (AppBarButton)GetTemplateChild("FastForward_Button");
                fastForwardButton.AddHandler(PointerPressedEvent, new PointerEventHandler((s, e) => FastForwardButton_PointerPressed?.Invoke(s, e)), true);
                fastForwardButton.AddHandler(PointerReleasedEvent, new PointerEventHandler((s, e) => FastForwardButton_PointerReleased?.Invoke(s, e)), true);
                fastForwardButton.AddHandler(PointerCanceledEvent, new PointerEventHandler((s, e) => FastForwardButton_PointerReleased?.Invoke(s, e)), true);
                fastForwardButton.AddHandler(PointerCaptureLostEvent, new PointerEventHandler((s, e) => FastForwardButton_PointerReleased?.Invoke(s, e)), true);
            }

            this.LogByObject("监听封面按钮的点击事件");
            {
                var coverButton = (Button)GetTemplateChild("Cover_Button");
                coverButton.Click += (s, e) => CoverButton_Click?.Invoke(this, e);
            }

            this.LogByObject("监听播放速率的进度更改事件");
            {
                _playbackRateSlider = (Slider)GetTemplateChild("PlaybackRate_Slider");
                _playbackRateSlider.Value = 1;
                _playbackRateSlider.ValueChanged += (s, e) => RateValueChanged?.Invoke(this, e);
            }

            this.LogByObject("监听单曲循环开关的切换事件");
            {
                var loopingToggle = (ToggleSwitch) GetTemplateChild("SingleLooping_ToggleSwitch");
                loopingToggle.IsOn = setting.SingleLoopingModeEnable;
                loopingToggle.Toggled += (s, e) => SingleLoopingToggled?.Invoke(s, e);
            }

            this.LogByObject("监听循环次数的进度更改事件");
            {
                var loppingTimesTextBlock = (TextBlock) GetTemplateChild("LoppingTimes_TextBlock");
                var loopingTimes = (int) setting.LoopingTimes;
                loppingTimesTextBlock.Text = loopingTimes > 0 ? loopingTimes.ToString() : "∞";

                _loopingTimesSlider = (Slider) GetTemplateChild("LoppingTimes_Slider");
                _loopingTimesSlider.Value = setting.LoopingTimes;
                _loopingTimesSlider.IsEnabled = OtherSettingProperties.Current.IsPaid;
                _loopingTimesSlider.ValueChanged += (s, e) =>
                {
                    var value = (int) e.NewValue;
                    loppingTimesTextBlock.Text = value > 0 ? value.ToString() : "∞";
                    LoopingTimesValueChanged?.Invoke(s, e);
                };
            }
        }

        private void OtherSetting_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OtherSettingProperties settings = (OtherSettingProperties) sender;
            switch (e.PropertyName)
            {
                case nameof(settings.IsPaid):
                    _loopingTimesSlider.IsEnabled = settings.IsPaid;

                    if (settings.IsPaid)
                        settings.PropertyChanged -= OtherSetting_PropertyChanged;
                    break;
            }
        }
    }
}
