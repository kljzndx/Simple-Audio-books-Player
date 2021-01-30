using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using HappyStudio.Parsing.Subtitle.Interfaces;
using HappyStudio.Subtitle.Control.Interface;
using HappyStudio.Subtitle.Control.Interface.Models.Extensions;
using HappyStudio.Subtitle.Control.Interface.Events;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;
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
        private readonly SubtitlePreviewSettingProperties _subtitleSettings = SubtitlePreviewSettingProperties.Current;
        private readonly OtherSettingProperties _otherSettings = OtherSettingProperties.Current;

        private readonly PlaybackListViewModel _vm;
        private readonly PlaybackListDataServer _listServer = PlaybackListDataServer.Current;
        private bool _needReposition;

        private int _readingTimes;
        private List<string> _SubtitleStringList;

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

            _settings.PropertyChanged += Settings_PropertyChanged;
            PlayerNotifier.CurrentItemChanged += PlayerNotifier_CurrentItemChanged;
            PlayerNotifier.PositionChanged += PlayerNotifier_PositionChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            _settings.PropertyChanged -= Settings_PropertyChanged;
            PlayerNotifier.CurrentItemChanged -= PlayerNotifier_CurrentItemChanged;
            PlayerNotifier.PositionChanged -= PlayerNotifier_PositionChanged;
        }

        private void AutoSplit()
        {
            bool b = String.IsNullOrEmpty(SplitSymbols_TextBox.Text);
            var list = My_ScrollSubtitlePreview.Source.ToList();
            for (int i = 0; i < list.Count; i++)
            {
                var line = list[i];
                var str = _SubtitleStringList[i];
                if (b)
                {
                    line.Content = str;
                }
                else
                {
                    var contents = str.Split(SplitSymbols_TextBox.Text.ToArray());
                    line.Content = String.Join("\r\n", contents);
                }
            }
        }

        private void SwitchSubtitleUi(List<ISubtitleLine> lines)
        {
            if (lines == null)
            {
                My_ScrollSubtitlePreview.Visibility = Visibility.Collapsed;
                SubtitleLoadError_StackPanel.Visibility = Visibility.Visible;
            }
            else
            {
                My_ScrollSubtitlePreview.Visibility = Visibility.Visible;
                SubtitleLoadError_StackPanel.Visibility = Visibility.Collapsed;

                My_ScrollSubtitlePreview.Source = lines.ToLineUiList();
            }
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_settings.ListWidth):
                    PlaybackList_Grid.Width = _settings.ListWidth;
                    break;
            }
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
            await _listServer.PlayTo(_vm.PlaybackListSource.IndexOf((MusicFile) e.ClickedItem));
        }

        private void PlaybackList_ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!e.AddedItems.Any())
                return;

            PlaybackList_ListView.ScrollIntoView(e.AddedItems.First());
        }

        private async void PlayerNotifier_CurrentItemChanged(object sender, MusicFile e)
        {
            if (e is null)
                return;

            PlaybackList_ListView.SelectedItem = e;

            var lines = await _vm.GetSubtitleLines();
            SwitchSubtitleUi(lines);
        }

        private void PlayerNotifier_PositionChanged(object sender, PlayerPositionChangeEventArgs e)
        {
            if (e.IsUser || _needReposition)
            {
                _needReposition = false;
                My_ScrollSubtitlePreview.Reposition(e.Position);
            }
            else
                My_ScrollSubtitlePreview.Refresh(e.Position);
        }

        private void My_ScrollSubtitlePreview_OnSourceChanged(object sender, IEnumerable<ISubtitleLine> e)
        {
            _needReposition = true;

            _SubtitleStringList = My_ScrollSubtitlePreview.Source.Select(l => l.Content).ToList();

            if (!String.IsNullOrEmpty(SplitSymbols_TextBox.Text))
                AutoSplit();
        }

        private void My_ScrollSubtitlePreview_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var item = e.ClickedItem as ISubtitleLine;
            if (item is null)
                return;

            PlayerNotifier.RequestChangePosition(item.StartTime);
        }

        private void ShowList_Button_OnClick(object sender, RoutedEventArgs e)
        {
            PlaybackList_Grid.Visibility = Visibility.Visible;
            SubtitlePreview_Grid.Visibility = Visibility.Collapsed;
        }

        private void ShowPreview_Button_OnClick(object sender, RoutedEventArgs e)
        {
            PlaybackList_Grid.Visibility = Visibility.Collapsed;
            SubtitlePreview_Grid.Visibility = Visibility.Visible;
        }

        private void PlaybackListPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize.Width > 640)
                PlaybackList_Grid.Width = _settings.ListWidth;
            else
                PlaybackList_Grid.Width = Double.NaN;
        }

        private void SplitSymbols_TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            AutoSplit();
        }

        private void My_ScrollSubtitlePreview_OnRefreshed(object sender, SubtitlePreviewRefreshedEventArgs e)
        {
            if (!_subtitleSettings.IsRereadingModeEnable || e.OldLine is null || e.NewLine is null ||
                // 判断NewLine是不是OldLine的下一句歌词，如不做判断将会导致死循环
                My_ScrollSubtitlePreview.Source.SkipWhile(l => l != e.OldLine).Skip(1).FirstOrDefault() != e.NewLine)
                return; 

            if (_readingTimes < _subtitleSettings.RereadingTimes)
            {
                _readingTimes++;
                PlayerNotifier.RequestChangePosition(e.OldLine.StartTime);
            }
            else
            {
                _readingTimes = 0;
            }
        }

        private async void ReScan_Button_Click(object sender, RoutedEventArgs e)
        {
            SwitchSubtitleUi(await _vm.GetSubtitleLines(true));
        }
    }
}
