using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SidePages;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views.SidePages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    [PageTitle("SettingsPage")]
    public sealed partial class SettingsPage : Page
    {
        private readonly SettingsViewModel _viewModel;

        public SettingsPage()
        {
            this.InitializeComponent();
            _viewModel = (SettingsViewModel) DataContext;
        }

        private async void ManageLocationOfScan_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await ManageLocationOfScan_ContentDialog.ShowAsync();
        }

        private async void ManageLocationOfScan_ContentDialog_OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if (_viewModel.MusicLibrary != null)
                return;

            _viewModel.MusicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            _viewModel.MusicLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
        }

        private async void AddLocation_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await _viewModel.MusicLibrary.RequestAddFolderAsync();
        }

        private async void Locations_ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var folder = e.ClickedItem as StorageFolder;
            if (folder is null)
                return;

            await _viewModel.MusicLibrary.RequestRemoveFolderAsync(folder);
        }

        private async void MusicLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await MusicLibraryDataServiceManager.Current.ScanFiles();
            });
        }
    }
}
