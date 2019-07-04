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
    public sealed partial class PlayRecordPage : Page
    {
        private readonly PlayRecordViewModel _viewModel;

        public PlayRecordPage()
        {
            this.InitializeComponent();
            _viewModel = (PlayRecordViewModel) DataContext;
        }

        private void AutoSetItemSize()
        {
            var itemsPanelRoot = Main_GridView.ItemsPanelRoot as ItemsWrapGrid;
            if (itemsPanelRoot == null)
                return;

            int temp = (int) Math.Ceiling(this.ActualWidth / 300);
            itemsPanelRoot.ItemWidth = this.ActualWidth / (temp > 0 ? temp : 1);
        }

        private void PlayRecordPage_OnLoaded(object sender, RoutedEventArgs e)
        {
            AutoSetItemSize();
        }

        private void PlayRecordPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            AutoSetItemSize();
        }

        private async void Main_GridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var record = e.ClickedItem as PlaybackRecordDTO;
            if (record == null)
                return;

            await PlaybackListDataServer.Current.SetSource(record);
        }
    }
}
