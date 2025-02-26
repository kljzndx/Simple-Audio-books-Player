﻿using SimpleAudioBooksPlayer.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Playback;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using NLog;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Log.Models;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.Extensions;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer
{
    /// <summary>
    /// 提供特定于应用程序的行为，以补充默认的应用程序类。
    /// </summary>
    sealed partial class App : Application
    {
        public static readonly MediaPlayer MediaPlayer = new MediaPlayer();

        private ResourceLoader _notificationStrings;

        private bool _canRefreshData = true;
        private ApplicationTheme _currentTheme;

        private readonly Logger _logger;
        
        /// <summary>
        /// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
        /// 已执行，逻辑上等同于 main() 或 WinMain()。
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;

            _logger = LoggerService.GetLogger(LoggerMembers.App);
            LogExtension.SetupLogger(typeof(ClassListDataService).Assembly, LoggerMembers.Service);
            LogExtension.SetupLogger(typeof(App).Assembly, LoggerMembers.Ui);
        }

        /// <summary>
        /// 在应用程序由最终用户正常启动时进行调用。
        /// 将在启动应用程序以打开特定文件等情况下使用。
        /// </summary>
        /// <param name="e">有关启动请求和过程的详细信息。</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
            _currentTheme = RequestedTheme;

            Frame rootFrame = Window.Current.Content as Frame;

            // 不要在窗口已包含内容时重复应用程序初始化，
            // 只需确保窗口处于活动状态
            if (rootFrame == null)
            {
                // 创建要充当导航上下文的框架，并导航到第一页
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: 从之前挂起的应用程序加载状态
                }

                // 将框架放在当前窗口中
                Window.Current.Content = rootFrame;
                Window.Current.Activated += Window_Activated;
                _notificationStrings = ResourceLoader.GetForCurrentView("Notifications");
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // 当导航堆栈尚未还原时，导航到第一页，
                    // 并通过将所需信息作为导航参数传入来配置
                    // 参数
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // 确保当前窗口处于活动状态
                Window.Current.Activate();
            }
        }

        protected override void OnBackgroundActivated(BackgroundActivatedEventArgs args)
        {
            var deferral = args.TaskInstance.GetDeferral();

            if (args.TaskInstance.Task.Name == SettingsPage.timedExitTaskName
                && DateTime.Now > OtherSettingProperties.Current.ExitTime)
            {
                
                _logger.Info("定时退出任务已触发，正在退出应用");
                args.TaskInstance.Task.Unregister(true);

                if (MediaPlayer.PlaybackSession != null && MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
                    MediaPlayer.Pause();

                Exit();
            }

            deferral.Complete();
        }

        private async void Window_Activated(object sender, WindowActivatedEventArgs e)
        {
            Window window = sender as Window;
            if (window is null)
                return;

            if (_canRefreshData && e.WindowActivationState != CoreWindowActivationState.Deactivated)
            {
                _canRefreshData = false;

                await window.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (_currentTheme != RequestedTheme)
                    {
                        NotificationNotifier.RequestShow(_notificationStrings.GetString("SwitchingTheme"));
                        _currentTheme = RequestedTheme;
                        ThemeChangeEvent.ReportThemeChange(_currentTheme);
                    }

                    NotificationNotifier.RequestShow(_notificationStrings.GetString("LoadingData"));
                    await ClassListDataServer.Current.Init();
                    await FileGroupDataServer.Current.Init();
                    await PlaybackRecordDataServer.Current.Init();
                    NotificationNotifier.RequestHide();
                    await PlaybackListDataServer.Current.Init();

                    NotificationNotifier.RequestShow(_notificationStrings.GetString("ScanningFolders"));
                    await FileGroupDataServer.Current.ScanFolders();
                    NotificationNotifier.RequestHide();
                });

                _canRefreshData = true;
            }
        }

        /// <summary>
        /// 导航到特定页失败时调用
        /// </summary>
        ///<param name="sender">导航失败的框架</param>
        ///<param name="e">有关导航失败的详细信息</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// 在将要挂起应用程序执行时调用。  在不知道应用程序
        /// 无需知道应用程序会被终止还是会恢复，
        /// 并让内存内容保持不变。
        /// </summary>
        /// <param name="sender">挂起的请求的源。</param>
        /// <param name="e">有关挂起请求的详细信息。</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: 保存应用程序状态并停止任何后台活动
            deferral.Complete();
        }

        private async void App_UnhandledException(object sender, Windows.UI.Xaml.UnhandledExceptionEventArgs e)
        {
            e.Handled = true;

            await e.Exception.ShowErrorDialog(_logger);
        }
    }
}
