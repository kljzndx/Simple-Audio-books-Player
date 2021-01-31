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

            NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void MusicViewFrame_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType != typeof(GroupListPage) && e.SourcePageType != typeof(MusicListPage))
                return;

            e.Cancel = true;
            this.Frame.Navigate(e.SourcePageType, e.Parameter, e.NavigationTransitionInfo);
        }
    }
}