using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class GroupListViewModel : ViewModelBase
    {
        private readonly FileGroupDataServer _server = FileGroupDataServer.Current;
        private readonly FileOpenPicker _coverPicker;

        public GroupListViewModel()
        {
            Data = new ObservableCollection<FileGroupDTO>();

            _coverPicker = new FileOpenPicker();
            _coverPicker.FileTypeFilter.Add(".jpg");
            _coverPicker.FileTypeFilter.Add(".png");
            _coverPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;

            _server.DataLoaded += Server_DataLoaded;
            _server.DataAdded += Server_DataAdded;
            _server.DataRenamed += Server_DataRenamed;
            _server.DataRemoved += Server_DataRemoved;
        }

        public ObservableCollection<FileGroupDTO> Data { get; }

        public void RefreshData()
        {
            Data.Clear();
            foreach (var groupDto in _server.Data)
                Data.Add(groupDto);
        }

        public async Task SetUpCover(FileGroupDTO groupDto)
        {
            var file = await _coverPicker.PickSingleFileAsync();
            if (file is null)
                return;

            await _server.SetCover(groupDto, file);
        }

        private void Server_DataLoaded(object sender, IEnumerable<FileGroupDTO> e)
        {
            RefreshData();
        }

        private void Server_DataAdded(object sender, IEnumerable<FileGroupDTO> e)
        {
            foreach (var fileGroupDto in e)
                Data.Add(fileGroupDto);
        }

        private void Server_DataRenamed(object sender, FileGroupDTO e)
        {
            // 排序方法
        }

        private void Server_DataRemoved(object sender, IEnumerable<FileGroupDTO> e)
        {
            foreach (var fileGroupDto in e)
                Data.Remove(fileGroupDto);
        }
    }
}