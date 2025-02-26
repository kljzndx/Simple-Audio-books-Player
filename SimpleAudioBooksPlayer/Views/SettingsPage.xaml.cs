﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
using SimpleAudioBooksPlayer.ViewModels.SidePages;
using SimpleAudioBooksPlayer.Views.Controls.Dialog;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    [PageTitle("SettingsPage")]
    public sealed partial class SettingsPage : Page
    {
        public const string timedExitTaskName = "TimedExitTask";

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
            await GlobalDialogs.Current.ShowLibraryManageDialog();
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
