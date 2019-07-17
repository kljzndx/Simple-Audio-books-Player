using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class ClassListPage : Page
    {
        private static readonly CoreCursor NormalCursor = new CoreCursor(CoreCursorType.Arrow, 1);
        private static readonly CoreCursor BothArrowCursor = new CoreCursor(CoreCursorType.SizeWestEast, 1);

        private readonly ClassListViewModel _vm;
        private readonly ClassListViewSettings _settings = ClassListViewSettings.Current;

        public ClassListPage()
        {
            this.InitializeComponent();
            _vm = (ClassListViewModel) DataContext;

            NavigationCacheMode = NavigationCacheMode.Enabled;

            AddClass_Grid.Visibility = Visibility.Collapsed;
            RequestAddClass_Button.IsEnabled = false;
        }

        private void GroupList_Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType == typeof(GroupListPage))
                return;

            e.Cancel = true;
            Frame.Navigate(e.SourcePageType, e.Parameter, e.NavigationTransitionInfo);
        }

        private void ClassList_ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GroupList_Frame.Navigate(typeof(GroupListPage), e.AddedItems.First());
        }

        private void ClassList_ListView_ContainerContentChanging(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (_vm.Data.Any())
            {
                ClassList_ListView.SelectedIndex = 0;
                ClassList_ListView.ContainerContentChanging -= ClassList_ListView_ContainerContentChanging;
            }
        }

        private void ShowAddClassPanel_Button_OnClick(object sender, RoutedEventArgs e)
        {
            AddClass_Grid.Visibility = Visibility.Visible;
            ClassName_TextBox.Focus(FocusState.Pointer);

            ShowAddClassPanel_Button.Visibility = Visibility.Collapsed;
        }

        private void AddClassItem(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return;

            _vm.Add(name);

            ShowAddClassPanel_Button.Visibility = Visibility.Visible;
            ShowAddClassPanel_Button.Focus(FocusState.Pointer);

            AddClass_Grid.Visibility = Visibility.Collapsed;
            ClassName_TextBox.Text = String.Empty;
        }

        private void ClassName_TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            RequestAddClass_Button.IsEnabled = !String.IsNullOrWhiteSpace(ClassName_TextBox.Text);
        }

        private void ClassName_TextBox_OnKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
                AddClassItem(ClassName_TextBox.Text);
        }

        private void RequestAddClass_Button_OnClick(object sender, RoutedEventArgs e)
        {
            AddClassItem(ClassName_TextBox.Text);
        }
        private void Separator_Rectangle_OnManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            var newWidth = _settings.ListWidth + e.Delta.Translation.X;
            if (newWidth > 220 && newWidth < ActualWidth - 400)
                _settings.ListWidth = newWidth;
        }

        private void Separator_Rectangle_OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = BothArrowCursor;
        }

        private void Separator_Rectangle_OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = NormalCursor;
        }
    }
}
