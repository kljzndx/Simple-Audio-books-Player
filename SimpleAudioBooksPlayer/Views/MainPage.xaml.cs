using System;
using System.Linq;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.ViewModels.Events;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly SystemNavigationManager _navigationManager = SystemNavigationManager.GetForCurrentView();

        public MainPage()
        {
            this.InitializeComponent();
            TitleBar_Grid.Visibility = Visibility.Collapsed;
            CustomMediaPlayerElement.SetMediaPlayer(App.MediaPlayer);
            _navigationManager.BackRequested += NavigationManager_BackRequested;

            NotificationNotifier.ShowRequested += NotificationNotifier_ShowRequested;
            NotificationNotifier.HideRequested += NotificationNotifier_HideRequested;

            PlayerNotifier.PositionChangeRequested += PlayerNotifier_PositionChangeRequested;
        }

        private void NavigationManager_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Main_Frame.GoBack();
        }

        private void Main_Frame_OnNavigated(object sender, NavigationEventArgs e)
        {
            if (Main_Frame.CanGoBack)
            {
                TitleBar_Grid.Visibility = Visibility.Visible;
                Title_TextBlock.Text = PageTitleGetter.GetTitle(e.SourcePageType);
            }
            else
                TitleBar_Grid.Visibility = Visibility.Collapsed;
        }

        private void GoBack_Button_OnClick(object sender, RoutedEventArgs e)
        {
            Main_Frame.GoBack();
        }

        private void NotificationNotifier_ShowRequested(object sender, string e)
        {
            My_NotificationBar.Show(e);
        }

        private void NotificationNotifier_HideRequested(object sender, object e)
        {
            My_NotificationBar.Hide();
        }

        private void PlayerNotifier_PositionChangeRequested(object sender, TimeSpan e)
        {
            CustomMediaPlayerElement.SetPosition(e);
        }

        private void CustomMediaPlayerElement_OnCoverButton_Click(object sender, RoutedEventArgs e)
        {
            Main_Frame.Navigate(typeof(PlaybackListPage));
        }
    }
}
