using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using HappyStudio.UwpToolsLibrary.Auxiliarys;
using HappyStudio.UwpToolsLibrary.Information;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls.Dialog
{
    public sealed partial class UpdateLogDialog : UserControl
    {
        public UpdateLogDialog()
        {
            this.InitializeComponent();
        }

        private async void UpdateLogDialog_Loaded(object sender, RoutedEventArgs e)
        {
            if (OtherSettingProperties.Current.UpdateLogVersion == AppInfo.Version)
                return;

            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/UpdateLog.md"));
            var fileContent = await FileReader.ReadText(file, "GBK");

            Main_ReelDialog.Show(fileContent);
        }

        private void Main_ReelDialog_Closed(object sender, EventArgs e)
        {
            OtherSettingProperties.Current.UpdateLogVersion = AppInfo.Version;
        }
    }
}
