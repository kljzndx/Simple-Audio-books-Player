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
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GroupListPage : Page
    {
        private readonly GroupListViewModel _vm;
        private FileGroupDTO _tempGroup;

        public GroupListPage()
        {
            this.InitializeComponent();
            _vm = (GroupListViewModel) this.DataContext;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            _vm.RefreshData();
        }

        private void Main_GridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var group = e.ClickedItem as FileGroupDTO;
            if (group == null)
                return;

            Frame.Navigate(typeof(MusicListPage), group.Index);
        }

        private void Main_GridView_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var theElement = (FrameworkElement) e.OriginalSource;
            var group = theElement.DataContext as FileGroupDTO;
            if (group is null)
                return;

            _tempGroup = group;
            RightTap_MenuFlyout.ShowAt(Main_GridView, e.GetPosition(Main_GridView));
        }

        private async void Rename_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await RenameGroup_ContentDialog.ShowAsync();
        }

        private async void SetCover_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await _vm.SetUpCover(_tempGroup);
            _tempGroup = null;
        }

        private async void RenameGroup_ContentDialog_OnPrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            if (!String.IsNullOrWhiteSpace(GroupName_TextBox.Text))
                await FileGroupDataServer.Current.Rename(_tempGroup, GroupName_TextBox.Text);
        }
    }
}
