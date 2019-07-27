using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
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
        public const string timedExitTaskName = "TimedExitTask";

        private readonly ResourceLoader _notificationStrings = ResourceLoader.GetForCurrentView("Notifications");
        private readonly SettingsViewModel _viewModel;
        private readonly OtherSettingProperties _settings = OtherSettingProperties.Current;

        public SettingsPage()
        {
            this.InitializeComponent();
            _viewModel = (SettingsViewModel) DataContext;
        }

        #region LocationOfScan

        private async void ManageLocationOfScan_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await ManageLocationOfScan_ContentDialog.ShowAsync();
        }

        private async void ManageLocationOfScan_ContentDialog_OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            if (_viewModel.MusicLibrary != null)
                return;

            this.LogByObject("获取音乐库数据");
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
                NotificationNotifier.RequestShow(_notificationStrings.GetString("ScanningFiles"));
                await MusicLibraryDataServiceManager.Current.ScanFiles();
                NotificationNotifier.RequestHide();
            });
        }

        #endregion

        private async Task<bool> StartTimer()
        {
            var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name is timedExitTaskName);
            if (task != null)
            {
                this.LogByObject("定时退出任务已创建");
                return true;
            }

            this.LogByObject("开始创建定时退出任务");
            BackgroundTaskBuilder builder = new BackgroundTaskBuilder
            {
                Name = timedExitTaskName,
            };

            this.LogByObject("申请注册任务");
            var b = await BackgroundExecutionManager.RequestAccessAsync();
            if (b == BackgroundAccessStatus.Unspecified)
            {
                this.LogByObject("申请失败，原因：没权限");
                await MessageBox.ShowAsync("Cannot create timed Task", "Please Open Background permissions for this app in the Windows Settings --> privacy --> Background App",
                    new Dictionary<string, UICommandInvokedHandler>
                    {
                        { "Open Settings", async u => await Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-backgroundapps")) }
                    }, "Close");
                return false;
            }

            if (b == BackgroundAccessStatus.DeniedBySystemPolicy || b == BackgroundAccessStatus.DeniedByUser)
            {
                this.LogByObject("申请失败，原因：省电方案");
                await MessageBox.ShowAsync("Cannot create background task", "Close");
                return false;
            }

            this.LogByObject($"申请成功，正在设置 15 分钟循环定时");
            builder.SetTrigger(new TimeTrigger(15, false));
            this.LogByObject("注册定时退出任务");
            builder.Register();

            this.LogByObject("注册完成");
            return true;
        }

        private async void TimedExitMinutes_ComboBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int minutes = (int)e.AddedItems.First();
            if (minutes == 0)
            {
                var task = BackgroundTaskRegistration.AllTasks.Values.FirstOrDefault(t => t.Name is timedExitTaskName);
                if (task != null)
                {
                    this.LogByObject("取消定时退出任务");
                    task.Unregister(true);
                }
                return;
            }

            _settings.ExitTime = DateTime.Now.AddMinutes(minutes - 5);
            TimedExitTime_Run.Text = _settings.ExitTime.AddMinutes(5).ToString("HH:mm:ss");

            var success = await StartTimer();
            if (!success)
                TimedExitMinutes_ComboBox.SelectedIndex = 0;
        }
    }
}
