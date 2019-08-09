using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HappyStudio.Subtitle.Control.UWP.Models;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    [PageTitle("PlaybackListPage")]
    public sealed partial class PlaybackListPage : Page
    {
        private static readonly CoreCursor NormalCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        private static readonly CoreCursor BothArrowCursor = new CoreCursor(CoreCursorType.SizeWestEast, 1);

        private readonly PlaybackViewSettingProperties _settings = PlaybackViewSettingProperties.Current;

        private readonly PlaybackListViewModel _vm;
        private readonly PlaybackListDataServer _listServer = PlaybackListDataServer.Current;

        public PlaybackListPage()
        {
            this.InitializeComponent();
            _vm = (PlaybackListViewModel) this.DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (_vm.CurrentMusic != null)
                PlayerNotifier_CurrentItemChanged(null, _vm.CurrentMusic);

            PlayerNotifier.CurrentItemChanged += PlayerNotifier_CurrentItemChanged;
            PlayerNotifier.PositionChanged += PlayerNotifier_PositionChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            PlayerNotifier.CurrentItemChanged -= PlayerNotifier_CurrentItemChanged;
            PlayerNotifier.PositionChanged -= PlayerNotifier_PositionChanged;
        }

        private void Separator_Rectangle_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var newWidth = _settings.ListWidth + e.Delta.Translation.X;
            if (newWidth > 220 && newWidth < ActualWidth - 400)
                _settings.ListWidth = newWidth;
        }

        private void Separator_Rectangle_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = BothArrowCursor;
        }

        private void Separator_Rectangle_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = NormalCursor;
        }

        private async void PlaybackList_ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            await _listServer.PlayTo(_vm.PlaybackListSource.IndexOf((MusicFileDTO) e.ClickedItem));
        }

        private void PlaybackList_ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!e.AddedItems.Any())
                return;

            PlaybackList_ListView.ScrollIntoView(e.AddedItems.First());
        }

        private async void PlayerNotifier_CurrentItemChanged(object sender, MusicFileDTO e)
        {
            if (e is null)
                return;

            PlaybackList_ListView.SelectedItem = e;

            var lines = await _vm.GetSubtitleLines();
            if (lines == null)
            {
                My_ScrollSubtitlePreview.Visibility = Visibility.Collapsed;
                CannotFindSubtitle_TextBlock.Visibility = Visibility.Visible;
            }
            else
            {
                My_ScrollSubtitlePreview.Visibility = Visibility.Visible;
                CannotFindSubtitle_TextBlock.Visibility = Visibility.Collapsed;

                My_ScrollSubtitlePreview.SetSubtitle(lines);
            }
        }

        private void PlayerNotifier_PositionChanged(object sender, PlayerPositionChangeEventArgs e)
        {
            if (e.IsUser)
                My_ScrollSubtitlePreview.Reposition(e.Position);
            else
                My_ScrollSubtitlePreview.Refresh(e.Position);
        }

        private void My_ScrollSubtitlePreview_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as SubtitleLineUi;
            if (item is null)
                return;

            PlayerNotifier.RequestChangePosition(item.StartTime);
        }
    }
}
