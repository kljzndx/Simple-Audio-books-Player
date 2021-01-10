using System;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HappyStudio.Subtitle.Control.Interface;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.ItemTemplates
{
    public sealed partial class ScrollPreviewItemTemplate : UserControl
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
            nameof(Text), typeof(string), typeof(ScrollPreviewItemTemplate), new PropertyMetadata(String.Empty));

        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(ScrollPreviewItemTemplate),
            new PropertyMetadata(false, IsSelected_PropertyChanged));

        private readonly SubtitlePreviewSettingProperties _settings = SubtitlePreviewSettingProperties.Current;

        public event TypedEventHandler<ScrollPreviewItemTemplate, string> SelectionChanged;

        public ScrollPreviewItemTemplate()
        {
            this.InitializeComponent();

            _settings.PropertyChanged -= Settings_PropertyChanged;
            _settings.PropertyChanged += Settings_PropertyChanged;
            Main_TextBlock.FontSize = _settings.FontSize;
            Main_TextBlock.Opacity = _settings.FontOpacity;
        }

        public string Text
        {
            get { return (string) GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
        
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        private void ScrollPreviewItemTemplate_OnLoaded(object sender, RoutedEventArgs e)
        {
            _settings.PropertyChanged += Settings_PropertyChanged;
        }

        private void ScrollPreviewItemTemplate_OnUnloaded(object sender, RoutedEventArgs e)
        {
            _settings.PropertyChanged -= Settings_PropertyChanged;
        }

        private void ChangeStatus(bool isSelected)
        {
            Main_TextBlock.FontWeight = isSelected ? FontWeights.Bold : FontWeights.Normal;
            Main_TextBlock.FontSize = isSelected ? _settings.FontSize + 2 : _settings.FontSize;
            Main_TextBlock.Opacity = isSelected ? 1 : _settings.FontOpacity;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_settings.FontSize):
                    if (IsSelected)
                        Main_TextBlock.FontSize = _settings.FontSize + 2;
                    else
                        Main_TextBlock.FontSize = _settings.FontSize;
                    
                    break;
                case nameof(_settings.FontOpacity):
                    if (!IsSelected)
                        Main_TextBlock.Opacity = _settings.FontOpacity;
                    break;
            }
        }

        private void Main_TextBlock_OnSelectionChanged(object sender, RoutedEventArgs e)
        {
            SelectionChanged?.Invoke(this, Main_TextBlock.SelectedText);
        }

        private static void IsSelected_PropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var theObj = (ScrollPreviewItemTemplate) d;
            
            theObj.ChangeStatus((bool) e.NewValue);
        }
    }
}
