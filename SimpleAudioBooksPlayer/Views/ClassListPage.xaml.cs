using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
using SimpleAudioBooksPlayer.Views.Controls.Dialog;

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
        private bool _isMiniView;
        private ClassItemDTO _tempClass;

        public ClassListPage()
        {
            this.InitializeComponent();
            _vm = (ClassListViewModel) DataContext;

            NavigationCacheMode = NavigationCacheMode.Enabled;

            RequestAddClass_Button.IsEnabled = false;

            _vm.Server.DataLoaded += Server_DataLoaded;

            _settings.PropertyChanged += Settings_PropertyChanged;

            GlobalDialogs.RenameDialogSubmitted += RenameClass_Dialog_OnSubmitted;
            GlobalDialogs.GroupsPicked += MyGroupsPicker_Picked;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ClassList_Grid.Width = _settings.ListWidth;
            CheckPageSize();
        }

        private void CheckPageSize()
        {
            if (ActualWidth <= 640)
            {
                if (GroupList_Frame.Visibility == Visibility.Collapsed)
                    return;

                Separator_Rectangle.Visibility = Visibility.Collapsed;
                GroupList_Frame.Visibility = Visibility.Collapsed;

                ClassList_Grid.Width = Double.NaN;
                Grid.SetColumnSpan(ClassList_Grid, 3);

                _isMiniView = true;

                if (_vm.Data.Any())
                    if (ClassList_ListView.SelectedIndex >= 0)
                        GroupList_Frame.Navigate(typeof(GroupListPage), _vm.Data[ClassList_ListView.SelectedIndex]);
                    else
                        ClassList_ListView.SelectedIndex = 0;
            }
            else if (GroupList_Frame.Visibility == Visibility.Collapsed)
            {
                Separator_Rectangle.Visibility = Visibility.Visible;
                GroupList_Frame.Visibility = Visibility.Visible;

                ClassList_Grid.Width = _settings.ListWidth;
                Grid.SetColumnSpan(ClassList_Grid, 1);

                _isMiniView = false;

                if (_vm.Data.Any())
                    if (ClassList_ListView.SelectedIndex >= 0)
                        GroupList_Frame.Navigate(typeof(GroupListPage), _vm.Data[ClassList_ListView.SelectedIndex]);
                    else
                        ClassList_ListView.SelectedIndex = 0;

            }
            else if (_settings.ListWidth > ActualWidth - 400)
                _settings.ListWidth = ActualWidth - 400;
        }

        private void Server_DataLoaded(object sender, IEnumerable<ClassItemDTO> e)
        {
            if (!_isMiniView)
                ClassList_ListView.SelectedIndex = _settings.CurrentClassId;
        }

        private void Settings_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_settings.ListWidth):
                    ClassList_Grid.Width = _settings.ListWidth;
                    break;
            }
        }

        private void GroupList_Frame_OnNavigating(object sender, NavigatingCancelEventArgs e)
        {
            if (e.SourcePageType == typeof(GroupListPage) && !_isMiniView)
                return;

            e.Cancel = true;
            Frame.Navigate(e.SourcePageType, e.Parameter, e.NavigationTransitionInfo);
        }

        private void ClassList_ListView_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var classItem in _vm.Data.Where(c => c.IsSelected))
                classItem.IsSelected = false;

            if (e.AddedItems.Any())
            {
                var item = (ClassItemDTO) e.AddedItems.First();
                item.IsSelected = true;
                GroupList_Frame.Navigate(typeof(GroupListPage), item);
                _settings.CurrentClassId = ClassList_ListView.SelectedIndex;
            }
            else
                ClassList_ListView.SelectedIndex = _settings.CurrentClassId - 1;
        }

        private void AddClassItem(string name)
        {
            if (String.IsNullOrWhiteSpace(name))
                return;

            _vm.Add(name);
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

        private void ClassListPage_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            CheckPageSize();
        }

        private void ClassList_ListView_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.OriginalSource is FrameworkElement element && element.DataContext is ClassItemDTO classItem)
            {
                _tempClass = classItem;
                bool b = classItem != ClassListDataServer.All_ClassItem && 
                         classItem != ClassListDataServer.Unspecified_ClassItem;

                RightTap_MenuFlyout.ShowAt(ClassList_ListView, e.GetPosition(ClassList_ListView));
                Remove_MenuFlyoutItem.IsEnabled = b;
                AddItem_MenuFlyoutItem.IsEnabled = b;
            }
        }

        private async void Rename_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await GlobalDialogs.Current.ShowRenameDialog(_tempClass.Name);
        }

        private async void Remove_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await _vm.Server.Remove(_tempClass);
            _tempClass = null;
        }

        private async void RenameClass_Dialog_OnSubmitted(RenameDialog sender, string args)
        {
            await _vm.Server.Rename(_tempClass, args);
            _tempClass = null;
        }

        private void ClassListItemTemplate_OnDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Link;
        }

        private async void ClassListItemTemplate_OnDrop(object sender, DragEventArgs e)
        {
            var element = sender as FrameworkElement;
            var ci = element?.DataContext as ClassItemDTO;
            if (ci is null || e.DataView.AvailableFormats.All(s => s != StandardDataFormats.Text))
                return;

            var deferral = e.GetDeferral();

            var group = await e.DataView.GetTextAsync();
            int groupId = Int32.Parse(group);
            await FileGroupDataServer.Current.SetClass(FileGroupDataServer.Current.Data.First(g => g.Index == groupId), ci);

            deferral.Complete();
        }

        private async void AddItem_MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            await GlobalDialogs.Current.ShowGroupsPickerDialog();
        }

        private async void MyGroupsPicker_Picked(GroupsPicker sender, IList<FileGroupDTO> args)
        {
            await FileGroupDataServer.Current.SetClass(args, _tempClass);
        }
    }
}
