﻿using System;
using System.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private SystemNavigationManager _navigationManager = SystemNavigationManager.GetForCurrentView();

        public MainPage()
        {
            this.InitializeComponent();
            CustomMediaPlayerElement.SetMediaPlayer(App.MediaPlayer);
            _navigationManager.BackRequested += _navigationManager_BackRequested;
        }

        private void _navigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Main_Frame.GoBack();
        }

        private async void Button_OnClick(object sender, RoutedEventArgs e)
        {
            await Launcher.LaunchFolderAsync(ApplicationData.Current.LocalFolder);
        }

        private void Main_Frame_OnNavigated(object sender, NavigationEventArgs e)
        {
            _navigationManager.AppViewBackButtonVisibility = Main_Frame.CanGoBack
                ? AppViewBackButtonVisibility.Visible
                : AppViewBackButtonVisibility.Collapsed;
        }
    }
}
