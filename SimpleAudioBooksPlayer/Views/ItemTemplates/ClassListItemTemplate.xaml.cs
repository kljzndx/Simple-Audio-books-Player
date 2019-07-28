using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;

//https://go.microsoft.com/fwlink/?LinkId=234236 上介绍了“用户控件”项模板

namespace SimpleAudioBooksPlayer.Views.ItemTemplates
{
    public sealed partial class ClassListItemTemplate : UserControl
    {
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(
            nameof(Source), typeof(ClassItemDTO), typeof(ClassListItemTemplate), new PropertyMetadata(null));

        public ClassListItemTemplate()
        {
            this.InitializeComponent();
        }

        public ClassItemDTO Source
        {
            get => (ClassItemDTO) GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        private async void Submit_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await ClassListDataServer.Current.SetBackgroundColor(Source, Background_ColorPicker.Color);
        }

        private async void Clear_Button_OnClick(object sender, RoutedEventArgs e)
        {
            await ClassListDataServer.Current.SetBackgroundColor(Source, Colors.Transparent);
        }
    }
}
