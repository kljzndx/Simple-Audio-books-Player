using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.DataServer;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class MusicListViewModel : ViewModelBase
    {
        private readonly MusicFileDataServer _server = MusicFileDataServer.Current;
        private readonly MusicListSettingProperties _settings = MusicListSettingProperties.Current;

        private int _groupId;
        private MusicListSortMembers _sortMethod;

        public MusicListViewModel()
        {
            this.LogByObject("初始化排序方法列表");
            SorterMembers.Add(new MusicSorterUi<MusicFileDTO>("TrackNumber", MusicSortDeserialization.Deserialize(MusicListSortMembers.TrackId)));
            SorterMembers.Add(new MusicSorterUi<MusicFileDTO>("Title", MusicSortDeserialization.Deserialize(MusicListSortMembers.Name)));
            SorterMembers.Add(new MusicSorterUi<MusicFileDTO>("ModifyDate", MusicSortDeserialization.Deserialize(MusicListSortMembers.ModifyTime)));

            _server.DataAdded += Server_DataAdded;
            _server.DataRemoved += Server_DataRemoved;
            _server.DataUpdated += Server_DataUpdated;
        }

        public ObservableCollection<MusicFileDTO> Data { get; } = new ObservableCollection<MusicFileDTO>();
        public List<MusicSorterUi<MusicFileDTO>> SorterMembers { get; } = new List<MusicSorterUi<MusicFileDTO>>();

        public void RefreshData(int groupId)
        {
            if (groupId != _groupId)
            {
                this.LogByObject("加载并排序数据");
                _groupId = groupId;
                _sortMethod = _settings.SortMethod;

                IEnumerable<MusicFileDTO> source = _server.Data.Where(g => g.Group.Index == groupId)
                    .OrderBy(SorterMembers[(int) _sortMethod].KeySelector.Invoke);

                if (_settings.IsReverse)
                    source = source.Reverse();

                var data = source.ToList();

                Data.Clear();
                foreach (var fileDto in data)
                    Data.Add(fileDto);
            }

            if (_settings.SortMethod != _sortMethod)
                SortData(_settings.SortMethod);
        }

        public void SortData(MusicListSortMembers method)
        {
            var mu = SorterMembers[(int) method];
            this.LogByObject($"按 {mu.Name} 排序数据");
            _sortMethod = method;
            _settings.SortMethod = method;

            IEnumerable<MusicFileDTO> source = Data.OrderBy(mu.KeySelector.Invoke);

            if (_settings.IsReverse)
                source = source.Reverse();

            var data = source.ToList();

            for (var i = 0; i < data.Count; i++)
                Data.Move(Data.IndexOf(data[i]), i);
        }

        public void Reverse()
        {
            this.LogByObject("倒序排序数据");
            _settings.IsReverse = !_settings.IsReverse;

            var data = Data.Reverse().ToList();
            for (var i = 0; i < data.Count; i++)
                Data.Move(Data.IndexOf(data[i]), i);
        }

        private void Server_DataAdded(object sender, IEnumerable<MusicFileDTO> e)
        {
            var list = e.Where(m => m.Group.Index == _groupId).ToList();
            if (!list.Any())
                return;

            foreach (var fileDto in list)
                Data.Add(fileDto);

            SortData(_sortMethod);
            if (_settings.IsReverse)
                Reverse();
        }

        private void Server_DataRemoved(object sender, IEnumerable<MusicFileDTO> e)
        {
            foreach (var fileDto in e.Where(Data.Contains))
                Data.Remove(fileDto);
        }

        private void Server_DataUpdated(object sender, IEnumerable<MusicFileDTO> e)
        {
            SortData(_sortMethod);
            if (_settings.IsReverse)
                Reverse();
        }
    }
}