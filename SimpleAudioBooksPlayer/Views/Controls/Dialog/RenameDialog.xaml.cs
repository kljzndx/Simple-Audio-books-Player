using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class RenameDialog : UserControl
    {
        public event TypedEventHandler<RenameDialog, string> Submitted;

        public RenameDialog()
        {
            this.InitializeComponent();
        }

        public async Task Show(string oldName = null)
        {
            Rename_ContentDialog.IsPrimaryButtonEnabled = !String.IsNullOrWhiteSpace(oldName);
            NewName_TextBox.Text = oldName ?? String.Empty;

            await Rename_ContentDialog.ShowAsync();
        }

        private void Rename_ContentDialog_OnClosed(ContentDialog sender, ContentDialogClosedEventArgs args)
        {
            Submitted?.Invoke(this, NewName_TextBox.Text);
        }

        private void NewName_TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            Rename_ContentDialog.IsPrimaryButtonEnabled = !String.IsNullOrWhiteSpace(NewName_TextBox.Text);
        }

        private void NewName_TextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (!String.IsNullOrWhiteSpace(NewName_TextBox.Text) && e.Key == VirtualKey.Enter)
                Rename_ContentDialog.Hide();
        }
    }
}
