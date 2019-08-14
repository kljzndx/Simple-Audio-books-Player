using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HappyStudio.Subtitle.Control.UWP.Models;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.ItemTemplates
{
    public sealed partial class ScrollPreviewItemTemplate : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(SubtitleLineUi), typeof(ScrollPreviewItemTemplate), new PropertyMetadata(null));

        private readonly SubtitlePreviewSettingProperties _settings = SubtitlePreviewSettingProperties.Current;

        private SubtitleLineUi _currentSource;

        public event TypedEventHandler<ScrollPreviewItemTemplate, string> SelectionChanged;

        public ScrollPreviewItemTemplate()
        {
            this.InitializeComponent();
            Main_TextBlock.FontSize = _settings.FontSize;
            Main_TextBlock.Opacity = _settings.FontOpacity;
        }

        public SubtitleLineUi Source
        {
            get => (SubtitleLineUi) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private void ScrollPreviewItemTemplate_OnLoaded(object sender, RoutedEventArgs e)
        {
            this.DataContextChanged += ScrollPreviewItemTemplate_DataContextChanged;
            _settings.PropertyChanged += Settings_PropertyChanged;

            ScrollPreviewItemTemplate_DataContextChanged(null, null);
        }

        private void ScrollPreviewItemTemplate_OnUnloaded(object sender, RoutedEventArgs e)
        {
            if (_currentSource != null)
                _currentSource.PropertyChanged -= Source_PropertyChanged;

            _settings.PropertyChanged -= Settings_PropertyChanged;
            this.DataContextChanged -= ScrollPreviewItemTemplate_DataContextChanged;
        }

        private void ScrollPreviewItemTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (_currentSource != null)
                _currentSource.PropertyChanged -= Source_PropertyChanged;

            _currentSource = Source;

            if (_currentSource != null)
                _currentSource.PropertyChanged += Source_PropertyChanged;
        }

        private void Source_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(Source.IsSelected):
                    if (Source.IsSelected)
                    {
                        Main_TextBlock.FontWeight = FontWeights.Bold;
                        Main_TextBlock.FontSize = _settings.FontSize + 2;
                        Main_TextBlock.Opacity = 1;
                    }
                    else
                    {
                        Main_TextBlock.FontWeight = FontWeights.Normal;
                        Main_TextBlock.FontSize = _settings.FontSize;
                        Main_TextBlock.Opacity = _settings.FontOpacity;
                    }
                    break;
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_settings.FontSize):
                    if (Source.IsSelected)
                        Main_TextBlock.FontSize = _settings.FontSize + 2;
                    else
                        Main_TextBlock.FontSize = _settings.FontSize;
                    
                    break;
                case nameof(_settings.FontOpacity):
                    if (!Source.IsSelected)
                        Main_TextBlock.Opacity = _settings.FontOpacity;
                    break;
            }
        }

        private void Main_TextBlock_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionChanged?.Invoke(this, Main_TextBlock.SelectedText);
        }
    }
}
