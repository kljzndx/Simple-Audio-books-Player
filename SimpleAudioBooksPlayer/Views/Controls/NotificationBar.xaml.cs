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

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.Controls
{
    public sealed partial class NotificationBar : UserControl
    {
        public NotificationBar()
        {
            this.InitializeComponent();
        }

        public void Show(string text)
        {
            Fold_Storyboard.Stop();
            Root_Border.Visibility = Visibility.Visible;
            Main_TextBlock.Text = text ?? String.Empty;
            Extend_Storyboard.Begin();
        }

        public void Hide()
        {
            Fold_Storyboard.Begin();
        }

        private void Fold_Storyboard_OnCompleted(object sender, object e)
        {
            Root_Border.Visibility = Visibility.Collapsed;
        }
    }
}
