using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MusicListPage : Page
    {
        private MusicListSettingProperties _settings = MusicListSettingProperties.Current;
        private MusicListViewModel _vm;
        public MusicListPage()
        {
            this.InitializeComponent();
            _vm = (MusicListViewModel) this.DataContext;
            Sorter_ListView.SelectedIndex = (int) _settings.SortMethod;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is null)
                return;

            var groupId = (int) e.Parameter;
            _vm.RefreshData(groupId);
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
    }
}
