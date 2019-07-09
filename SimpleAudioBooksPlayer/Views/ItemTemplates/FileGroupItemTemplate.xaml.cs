using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;

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
            get => (FileGroupDTO) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private async void FileGroupItemTemplate_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (Source != null)
            {
                Cover_Image.Source = await Source.GetCover();
                Source.CoverChanged += Source_CoverChanged;
            }
        }

        private async void Source_CoverChanged(object sender, object e)
        {
            Cover_Image.Source = await Source.GetCover();
        }

        private void ControlButtonFadeOut_Storyboard_OnCompleted(object sender, object e)
        {
            ControlButton_StackPanel.Visibility = Visibility.Collapsed;
        }

        private void Cover_Grid_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ControlButton_StackPanel.Visibility = Visibility.Visible;
            ControlButtonFadeIn_Storyboard.Begin();
        }

        private void Cover_Grid_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            ControlButtonFadeOut_Storyboard.Begin();
        }

        private async void Play_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await PlaybackListDataServer.Current.SetSource(Source);
        }

        private void More_Button_OnClick(object sender, RoutedEventArgs e)
        {
            GroupListMoreMenuNotifier.RequestShowMoreMenu(More_Button);
        }

        private void Cover_Image_OnTapped(object sender, TappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Mouse && ControlButton_StackPanel.Visibility == Visibility.Collapsed)
            {
                e.Handled = true;
                ControlButton_StackPanel.Visibility = Visibility.Visible;
                ControlButtonFadeIn_Storyboard.Begin();
            }
        }

        private void Cover_Image_OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Copy;
        }

        private async void Cover_Image_OnDrop(object sender, DragEventArgs e)
        {
            var items = await e.DataView.GetStorageItemsAsync();
            var file = items.FirstOrDefault(si => si.IsOfType(StorageItemTypes.File)) as StorageFile;
            if (file is null || (file.FileType.ToLower() != ".jpg" && file.FileType.ToLower() != ".png"))
                return;

            await FileGroupDataServer.Current.SetCover(Source, file);
        }
    }
}
