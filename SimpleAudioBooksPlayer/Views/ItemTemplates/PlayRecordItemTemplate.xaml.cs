using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.DTO;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.ItemTemplates
{
    public sealed partial class PlayRecordItemTemplate : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(PlaybackRecordDTO), typeof(PlayRecordItemTemplate), new PropertyMetadata(null));

        public PlayRecordItemTemplate()
        {
            this.InitializeComponent();
            Title_TextBlock.FontSize += 1;
            this.DataContextChanged += PlayRecordItemTemplate_DataContextChanged;
        }

        public PlaybackRecordDTO Source
        {
            get => (PlaybackRecordDTO) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private async void PlayRecordItemTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Source != null && Cover_Image.Source is null)
            {
                Cover_Image.Source = await Source.Group.GetCover();
                Source.Group.CoverChanged += Group_CoverChanged;
            }
        }

        private async void Group_CoverChanged(object sender, object e)
        {
            Cover_Image.Source = await Source.Group.GetCover();
        }
    }
}
