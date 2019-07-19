using System;
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
    }
}
