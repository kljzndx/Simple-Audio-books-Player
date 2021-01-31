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
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.Attributes;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.Events;
using SimpleAudioBooksPlayer.Views.Controls.Dialog;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;
using System.Threading.Tasks;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace SimpleAudioBooksPlayer.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    [PageTitle("GroupListPage")]
    public sealed partial class GroupListPage : Page
    {
        private readonly GroupListViewModel _vm;
        private FileGroupDTO _tempGroup;
        private readonly OtherSettingProperties _otherSettings = OtherSettingProperties.Current;

        public GroupListPage()
        {
            this.InitializeComponent();
            _vm = (GroupListViewModel) this.DataContext;

            Sorter_ComboBox.SelectedIndex = (int) _vm.Settings.SortMethod;
            Main_GridView.ItemsSource = _vm.Data;

            GroupListMoreMenuNotifier.ShowMoreMenuRequested += GroupListMoreMenuNotifier_ShowMoreMenuRequested;

            GlobalDialogs.RenameDialogSubmitted += RenameGroup_Dialog_OnSubmitted;
            GlobalDialogs.ClassPicked += MyClassPicker_OnPicked;
            GlobalDialogs.GroupsPicked += GlobalDialogs_GroupsPicked;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is ClassItemDTO classItem)
            {
                Import_Button.IsEnabled = classItem != ClassListDataServer.All_ClassItem && classItem != ClassListDataServer.Unspecified_ClassItem;

                _vm.RefreshData(classItem);
            }
        }

        private void Main_GridView_OnItemClick(object sender, ItemClickEventArgs e)
        {
            var group = e.ClickedItem as FileGroupDTO;
            if (group == null)
                return;

            Frame.Navigate(typeof(MusicListPage), group.Index);
        }

        private void Main_GridView_OnRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            var theElement = (FrameworkElement) e.OriginalSource;
            var group = theElement.DataContext as FileGroupDTO;
            if (group is null)
                return;

            _tempGroup = group;
            RightTap_MenuFlyout.ShowAt(Main_GridView, e.GetPosition(Main_GridView));
        }

        private void GroupListMoreMenuNotifier_ShowMoreMenuRequested(FrameworkElement sender, object e)
        {
            var dto = sender.DataContext as FileGroupDTO;
            if (dto == null)
                return;

            _tempGroup = dto;
            RightTap_MenuFlyout.ShowAt(sender);
        }

        private async void Rename_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await GlobalDialogs.Current.ShowRenameDialog(this, _tempGroup.Name);
        }

        private async void SetCover_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await _vm.SetUpCover(_tempGroup);
            _tempGroup = null;
        }

        private async void RenameGroup_Dialog_OnSubmitted(object sender, string args)
        {
            if (sender != this)
                return;

            if (!String.IsNullOrWhiteSpace(args))
                await FileGroupDataServer.Current.Rename(_tempGroup, args);

            _tempGroup = null;
        }

        private void Sorter_ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Sorter_ComboBox.SelectedIndex == (int)_vm.Settings.SortMethod)
                return;

            var sortMethod = _vm.SorterMembers[Sorter_ComboBox.SelectedIndex];

            this.LogByObject($"切换排序方法到 {sortMethod.Name}");
            _vm.Sort(sortMethod);
        }

        private void Search_TextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (String.IsNullOrWhiteSpace(Search_TextBox.Text))
                Main_GridView.ItemsSource = _vm.Data;
            else
            {
                this.LogByObject("搜索数据");
                Main_GridView.ItemsSource = _vm.Data.Where(g => g.Name.ToLower().Contains(Search_TextBox.Text.ToLower())).ToList();
            }
        }

        private async void SetClass_MenuFlyoutItem_OnClick(object sender, RoutedEventArgs e)
        {
            await GlobalDialogs.Current.ShowClassPickerDialog(this);
        }

        private async void MyClassPicker_OnPicked(object sender, ClassItemDTO args)
        {
            if (sender != this)
                return;

            if (args != null)
                await FileGroupDataServer.Current.SetClass(_tempGroup, args);
            _tempGroup = null;
        }

        private void Main_GridView_OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            var data = (FileGroupDTO) e.Items.First();
            e.Data.SetText(data.Index.ToString());
        }

        private async void Import_Button_Click(object sender, RoutedEventArgs e)
        {
            await GlobalDialogs.Current.ShowGroupsPickerDialog(this);
        }

        private async void GlobalDialogs_GroupsPicked(object sender, IList<FileGroupDTO> args)
        {
            if (sender != this)
                return;

            await FileGroupDataServer.Current.SetClass(args, _vm.CurrentClass);
        }

        private async void ManageLocationOfScan_Button_Click(object sender, RoutedEventArgs e)
        {
            await GlobalDialogs.Current.ShowLibraryManageDialog();
        }

        private void CloseTip_Button_Click(object sender, RoutedEventArgs e)
        {
            _otherSettings.IsShowWelcomeTip = false;
        }
    }
}
