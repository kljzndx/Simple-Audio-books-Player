using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.Models.DTO;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
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
    public sealed partial class GlobalDialogs : UserControl
    {
        public static GlobalDialogs Current;

        public static event EventHandler<ClassItemDTO> ClassPicked;
        public static event EventHandler<IList<FileGroupDTO>> GroupsPicked;
        public static event EventHandler<string> RenameDialogSubmitted;

        private object _tempSender;

        public GlobalDialogs()
        {
            if (Current == null)
                Current = this;
            else 
                throw new Exception("only exist singgle instance");

            this.InitializeComponent();
        }

        private async Task RunInUiProcess(DispatchedHandler callback)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, callback);
        }

        public async Task ShowLibraryManageDialog()
        {
            await RunInUiProcess(async () =>
            {
                await _LibraryManageDialog.ShowAsync();
            });
        }

        public async Task ShowRenameDialog(object sender, string oldName)
        {
            _tempSender = sender;
            await RunInUiProcess(async () =>
            {
                await _RenameDialog.Show(oldName);
            });
        }

        public async Task ShowGroupsPickerDialog(object sender)
        {
            _tempSender = sender;
            await RunInUiProcess(async () =>
            {
                await _GroupsPicker.Show();
            });
        }

        public async Task ShowClassPickerDialog(object sender)
        {
            _tempSender = sender;
            await RunInUiProcess(async () =>
            {
                await _ClassPicker.Show();
            });
        }

        private void ClassPicker_Picked(ClassPicker sender, ClassItemDTO args)
        {
            ClassPicked?.Invoke(_tempSender, args);
        }

        private void GroupsPicker_Picked(GroupsPicker sender, IList<FileGroupDTO> args)
        {
            GroupsPicked?.Invoke(_tempSender, args);
        }

        private void RenameDialog_Submitted(RenameDialog sender, string args)
        {
            RenameDialogSubmitted?.Invoke(_tempSender, args);
        }
    }
}
