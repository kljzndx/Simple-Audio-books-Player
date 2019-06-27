using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
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
            SorterMembers.Add(new MusicSorterUi<MusicFileDTO>("TrackNumber", MusicSortDeserialization.Deserialize(MusicListSortMembers.TrackId)));
            SorterMembers.Add(new MusicSorterUi<MusicFileDTO>("Title", MusicSortDeserialization.Deserialize(MusicListSortMembers.Name)));
            SorterMembers.Add(new MusicSorterUi<MusicFileDTO>("ModifyDate", MusicSortDeserialization.Deserialize(MusicListSortMembers.ModifyTime)));
        }

        public ObservableCollection<MusicFileDTO> Data { get; } = new ObservableCollection<MusicFileDTO>();
        public List<MusicSorterUi<MusicFileDTO>> SorterMembers { get; } = new List<MusicSorterUi<MusicFileDTO>>();

        public void RefreshData(int groupId)
        {
            if (groupId != _groupId)
            {
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
                SortData();
        }

        private void SortData()
        {
            _sortMethod = _settings.SortMethod;

            IEnumerable<MusicFileDTO> source = Data.OrderBy(SorterMembers[(int) _sortMethod].KeySelector.Invoke);

            if (_settings.IsReverse)
                source = source.Reverse();

            var data = source.ToList();

            for (var i = 0; i < data.Count; i++)
                Data.Move(Data.IndexOf(data[i]), i);
        }

        public void Reverse()
        {
            _settings.IsReverse = !_settings.IsReverse;

            var data = Data.Reverse().ToList();
            for (var i = 0; i < data.Count; i++)
                Data.Move(Data.IndexOf(data[i]), i);
        }
    }
}