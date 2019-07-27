using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    [PageTitle("MusicListPage")]
    public sealed partial class MusicListPage : Page
    {
        private MusicListSettingProperties _settings = MusicListSettingProperties.Current;
        private MusicListViewModel _vm;

        public MusicListPage()
        {
            this.InitializeComponent();
            _vm = (MusicListViewModel) this.DataContext;

            this.LogByObject($"初始化排序方法");
            Sorter_ListView.SelectedIndex = (int) _settings.SortMethod;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is null)
                return;

            var groupId = (int) e.Parameter;
            _vm.RefreshData(groupId);
        }

        private void Goto()
        {
            if (!Int32.TryParse(ItemId_TextBox.Text, out var result))
                return;

            int id = result < 1 ? 1 : (result <= _vm.Data.Count ? result : _vm.Data.Count);

            this.LogByObject($"跳转到 第 {id} 项");

            var item = _vm.Data[id - 1];
            Main_ListView.ScrollIntoView(item, ScrollIntoViewAlignment.Leading);

            var container = Main_ListView.ContainerFromItem(item);
            ((Control) container).Focus(FocusState.Keyboard);
        }

        private async void Main_ListView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var theItem = e.ClickedItem as MusicFileDTO;
            if (theItem == null)
                return;

            await PlaybackListDataServer.Current.SetSource(theItem);
        }

        private void Sorter_ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var method = (MusicListSortMembers) Sorter_ListView.SelectedIndex;

            if (_settings.SortMethod != method)
                _vm.SortData(method);
        }

        private void Go_Button_OnClick(object sender, RoutedEventArgs e)
        {
            Goto();
        }

        private void ItemId_TextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                Goto();
            }
        }
    }
}
