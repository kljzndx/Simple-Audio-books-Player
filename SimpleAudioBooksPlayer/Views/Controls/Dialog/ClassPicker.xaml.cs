using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.Dialog
{
    public sealed partial class ClassPicker : UserControl
    {
        private readonly ClassListDataServer _server = ClassListDataServer.Current;

        public event TypedEventHandler<ClassPicker, ClassItemDTO> Picked;
         
        public ClassPicker()
        {
            this.InitializeComponent();

            AddClass_Button.IsEnabled = false;

            _server.DataAdded += DataServer_DataAdded;
        }

        public async Task Show()
        {
            await Root_ContentDialog.ShowAsync();
        }

        private void ClassName_TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            AddClass_Button.IsEnabled = !String.IsNullOrWhiteSpace(ClassName_TextBox.Text);
        }

        private async void AddClass_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await _server.Add(ClassName_TextBox.Text);
            ClassName_TextBox.Text = String.Empty;
        }

        private void Main_ContentDialog_OnSecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Picked?.Invoke(this, Main_ListView.SelectedItem as ClassItemDTO);
        }

        private void DataServer_DataAdded(object sender, IEnumerable<ClassItemDTO> e)
        {
            if (Main_ListView.ItemsSource is IList<ClassItemDTO> list)
                foreach (var dto in e)
                    list.Add(dto);
        }

        private void Root_ContentDialog_OnOpened(ContentDialog sender, ContentDialogOpenedEventArgs args)
        {
            var allItem = ClassListDataServer.All_ClassItem;
            Main_ListView.ItemsSource = new ObservableCollection<ClassItemDTO>(_server.Data.Where(i => i != allItem).ToList());
        }

        private void Root_ContentDialog_OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            Main_ListView.ItemsSource = null;
        }
    }
}
