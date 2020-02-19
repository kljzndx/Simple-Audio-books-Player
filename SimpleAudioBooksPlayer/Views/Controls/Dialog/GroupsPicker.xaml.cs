using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.Models;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.Dialog
{
    public sealed partial class GroupsPicker : UserControl
    {
        private FileGroupBoxDataServer _dataServer = FileGroupBoxDataServer.Current;

        public GroupsPicker()
        {
            this.InitializeComponent();
        }

        public event TypedEventHandler<GroupsPicker, IList<FileGroupDTO>> Picked;

        public async Task Show()
        {
            Root_ContentDialog.ShowAsync();

            Loading_ProgressRing.IsActive = true;
            await _dataServer.Init();
            Main_ListView.SelectedIndex = -1;
            Loading_ProgressRing.IsActive = false;
        }

        public void Hide()
        {
            Root_ContentDialog.Hide();
        }

        private void Root_ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Picked?.Invoke(this, Main_ListView.SelectedItems.Cast<FileGroupDTO>().ToList());
        }

        private void SelectAll_Button_Click(object sender, RoutedEventArgs e)
        {
            var theObj = (Button) sender;
            var data = (FileGroupBox) theObj.DataContext;
            foreach (var item in data.Groups)
            {
                if (!Main_ListView.SelectedItems.Contains(item))
                    Main_ListView.SelectedItems.Add(item);
            }
        }
    }
}
