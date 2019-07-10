using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Views.SidePages;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MusicViewFrameworkPage : Page
    {
        public MusicViewFrameworkPage()
        {
            this.InitializeComponent();
        }

        private void Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType != typeof(MusicListPage))
                return;

            e.Cancel = true;
            this.Frame.Navigate(e.SourcePageType, e.Parameter);
        }

        private void More_MenuFlyout_OnOpened(object sender, object e)
        {
            foreach (MenuFlyoutItem item in More_MenuFlyout.Items)
                item.Text = ResourceLoader.GetForCurrentView((string) item.Tag).GetString("Title");
        }

        private void Settings_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            Root_SplitView.IsPaneOpen = true;
            if (SidePage_Frame.SourcePageType != typeof(SettingsPage))
                SidePage_Frame.Navigate(typeof(SettingsPage));
        }

        private void About_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            Root_SplitView.IsPaneOpen = true;
            if (SidePage_Frame.SourcePageType != typeof(AboutPage))
                SidePage_Frame.Navigate(typeof(AboutPage));
        }

        private void SidePage_Frame_OnNavigated(object sender, NavigationEventArgs e)
        {
            Title_TextBlock.Text = PageTitleGetter.GetTitle(e.SourcePageType);
        }
    }
}