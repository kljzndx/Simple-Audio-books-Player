using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Information;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.Attributes;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views.SidePages
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    [PageTitle("AboutPage")]
    public sealed partial class AboutPage : Page
    {
        private readonly string appName = AppInfo.Name;
        private readonly string appVersion = AppInfo.Version;
        private readonly string feedbackEmailTitle = ResourceLoader.GetForCurrentView("AboutPage").GetString("FeedbackEmailTitle");

        public AboutPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private async void SendFeedback_AppBarButton_OnClick(object sender, RoutedEventArgs e)
        {
            this.LogByObject("点击 ‘反馈’ 按钮");
            await EmailEx.SendAsync("kljzndx@outlook.com", $"{appName} {appVersion} {feedbackEmailTitle}", String.Empty);
        }

        //private async void GitHub_AppBarButton_Click(object sender, RoutedEventArgs e)
        //{
        //    await Launcher.LaunchUriAsync(new Uri("https://github.com/kljzndx/Simple-Audio-books-Player"));
        //}

        private async void OpenLogsFolder_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.LogByObject("点击 ‘查看日志’ 按钮");
            await Launcher.LaunchFolderAsync(ApplicationData.Current.TemporaryFolder);
        }

        private async void Review_AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            this.LogByObject("点击 ‘评分’ 按钮");
            await Launcher.LaunchUriAsync(new Uri("ms-windows-store://review/?ProductId=9N6406PNNRZS"));
        }
    }
}
