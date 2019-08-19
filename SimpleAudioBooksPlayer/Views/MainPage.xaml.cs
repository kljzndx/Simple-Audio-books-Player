using System;
using System.Linq;
using Windows.Services.Store;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private const string AddOnStoreId = "9P40N0Q3BFB0";

        private readonly SystemNavigationManager _navigationManager = SystemNavigationManager.GetForCurrentView();
        private readonly OtherSettingProperties _settings = OtherSettingProperties.Current;
        private readonly StoreContext _storeContext = StoreContext.GetDefault();

        public MainPage()
        {
            this.InitializeComponent();
            TitleBar_Grid.Visibility = Visibility.Collapsed;
            CustomMediaPlayerElement.SetMediaPlayer(App.MediaPlayer);
            _navigationManager.BackRequested += NavigationManager_BackRequested;

            NotificationNotifier.ShowRequested += NotificationNotifier_ShowRequested;
            NotificationNotifier.HideRequested += NotificationNotifier_HideRequested;

            CustomMediaPlayerElement.PositionChanged += (s, e) => PlayerNotifier.RaisePosition(e);
            PlayerNotifier.PositionChangeRequested += PlayerNotifier_PositionChangeRequested;

            BuyToolNotifier.BuyRequested += BuyToolNotifier_BuyRequested;
        }

        private async void MainPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            if (!_settings.PaymentChecked)
            {
                var result = await _storeContext.GetStoreProductsAsync(new[] {"Durable"}, new[] {AddOnStoreId});
                if (result.ExtendedError != null || !result.Products.Any())
                    return;

                _settings.IsPaid = result.Products.First().Value.IsInUserCollection;
                _settings.PaymentChecked = true;
            }
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

        private async void BuyToolNotifier_BuyRequested(object sender, EventArgs e)
        {
            var result = await _storeContext.RequestPurchaseAsync(AddOnStoreId);
            switch (result.Status)
            {
                case StorePurchaseStatus.Succeeded:
                case StorePurchaseStatus.AlreadyPurchased:
                    _settings.IsPaid = true;
                    break;
            }
        }

        private void CustomMediaPlayerElement_OnCoverButton_Click(object sender, RoutedEventArgs e)
        {
            Main_Frame.Navigate(typeof(PlaybackListPage));
        }
    }
}
