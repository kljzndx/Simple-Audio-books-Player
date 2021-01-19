﻿using System.Collections.Generic;
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

        public MusicListViewModel()
        {
            this.LogByObject("初始化排序方法列表");
            SorterMembers = new List<MusicSorterUi<MusicFile>>();
            SorterMembers.Add(new MusicSorterUi<MusicFile>("TrackNumber", MusicSortDeserialization.Deserialize(MusicListSortMembers.TrackId)));
            SorterMembers.Add(new MusicSorterUi<MusicFile>("Title", MusicSortDeserialization.Deserialize(MusicListSortMembers.Name)));
            SorterMembers.Add(new MusicSorterUi<MusicFile>("ModifyDate", MusicSortDeserialization.Deserialize(MusicListSortMembers.ModifyTime)));
        }

        public List<MusicSorterUi<MusicFile>> SorterMembers { get; }

        public ObservableCollection<MusicFile> Data => _server.Data;

        public async Task RefreshData(int groupId)
        {
            await _server.RefreshData(groupId);
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