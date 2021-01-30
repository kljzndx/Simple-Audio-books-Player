using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.FileModels;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class MusicListViewModel : ViewModelBase
    {
        private readonly MusicFileDataServer _server = MusicFileDataServer.Current;

        private bool _isRefreshing;

        public MusicListViewModel()
        {
            this.LogByObject("初始化排序方法列表");
            SorterMembers = new List<MusicSorterUi<MusicFile>>();
            SorterMembers.Add(new MusicSorterUi<MusicFile>("TrackNumber", MusicSortDeserialization.Deserialize(MusicListSortMembers.TrackId)));
            SorterMembers.Add(new MusicSorterUi<MusicFile>("Title", MusicSortDeserialization.Deserialize(MusicListSortMembers.Name)));
            SorterMembers.Add(new MusicSorterUi<MusicFile>("ModifyDate", MusicSortDeserialization.Deserialize(MusicListSortMembers.ModifyTime)));
        }

        public List<MusicSorterUi<MusicFile>> SorterMembers { get; }
        public bool IsForceScan { get; private set; }

        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => Set(ref _isRefreshing, value);
        }

        public ObservableCollection<MusicFile> Data => _server.Data;

        public async Task RefreshData(int groupId, bool isForceScan = false)
        {
            IsRefreshing = true;
            IsForceScan = isForceScan;

            await _server.RefreshData(groupId, isForceScan);

            IsRefreshing = false;
        }

        public void SortData(MusicListSortMembers method)
        {
            _server.SortData(method);
        }

        public void Reverse()
        {
            _server.Reverse();
        }
    }
}