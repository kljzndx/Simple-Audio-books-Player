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
    public sealed partial class FileGroupItemTemplate : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(FileGroupDTO), typeof(FileGroupItemTemplate), new PropertyMetadata(null));

        public FileGroupItemTemplate()
        {
            this.InitializeComponent();
            DataContextChanged += FileGroupItemTemplate_DataContextChanged;
        }

        public FileGroupDTO Source
        {
            get => (FileGroupDTO)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private async void FileGroupItemTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Cover_Image.Source is null && Source != null)
                Cover_Image.Source = await Source.GetCover();
        }

    }
}
