using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;
using Windows.ApplicationModel.Resources;
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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.Dialog
{
    public sealed partial class LibraryManageDialog : UserControl
    {
        private readonly ResourceLoader _notificationStrings = ResourceLoader.GetForCurrentView("Notifications");
        private StorageLibrary _musicLibrary;

        public LibraryManageDialog()
        {
            this.InitializeComponent();
        }

        public async Task ShowAsync()
        {
            await ManageLocationOfScan_ContentDialog.ShowAsync();
        }

        private async void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (_musicLibrary != null)
                return;

            this.LogByObject("获取音乐库数据");
            _musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            Locations_ListView.ItemsSource = _musicLibrary.Folders;
            _musicLibrary.DefinitionChanged += MusicLibrary_DefinitionChanged;
        }

        private async void AddLocation_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await _musicLibrary.RequestAddFolderAsync();
        }

        private async void Locations_ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var folder = e.ClickedItem as StorageFolder;
            if (folder is null)
                return;

            await _musicLibrary.RequestRemoveFolderAsync(folder);
        }

        private async void MusicLibrary_DefinitionChanged(StorageLibrary sender, object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                NotificationNotifier.RequestShow(_notificationStrings.GetString("ScanningFolders"));
                await FileGroupDataServer.Current.ScanFolders();
                NotificationNotifier.RequestHide();
            });
        }
    }
}
