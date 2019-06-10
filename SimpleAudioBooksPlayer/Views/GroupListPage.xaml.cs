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

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class GroupListPage : Page
    {
        private GroupListViewModel _vm;

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
    }
}
