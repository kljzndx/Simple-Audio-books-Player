using System.Collections.Generic;
using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class GroupListViewModel : ViewModelBase
    {
        private FileGroupDataServer _server = FileGroupDataServer.Current;

        public GroupListViewModel()
        {
            Data = new ObservableCollection<FileGroupDTO>();
            _server.DataLoaded += Server_DataLoaded;
        }

        public ObservableCollection<FileGroupDTO> Data { get; }

        public void RefreshData()
        {
            Data.Clear();
            foreach (var groupDto in _server.Data)
                Data.Add(groupDto);
        }

        private void Server_DataLoaded(object sender, IEnumerable<FileGroupDTO> e)
        {
            RefreshData();
        }
    }
}