﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class MusicListViewModel : ViewModelBase
    {
        private MusicFileDataServer _server;
        private ObservableCollection<MusicFileDTO> _data = new ObservableCollection<MusicFileDTO>();
        private int _groupId;

        public MusicListViewModel()
        {
            _server = MusicFileDataServer.Current;
            _server.DataAdded += Server_DataAdded;
            _server.DataRemoved += Server_DataRemoved;
        }

        public ObservableCollection<MusicFileDTO> Data
        {
            get => _data;
            set => Set(ref _data, value);
        }

        public void RefreshData(int groupId)
        {
            foreach (var fileDto in _server.Data.Where(g => g.GroupId == groupId))
                Data.Add(fileDto);

            _server.Data.CollectionChanged += Server_Data_CollectionChanged;
            _groupId = groupId;
        }

        private void Server_Data_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (MusicFileDTO item in e.NewItems)
                    if (item.GroupId == _groupId)
                        Data.Add(item);

            if (e.OldItems != null)
                foreach (MusicFileDTO item in e.OldItems)
                    if (Data.Contains(item))
                        Data.Remove(item);

        }

        private void Server_DataAdded(object sender, IEnumerable<MusicFileDTO> e)
        {
        }

        private void Server_DataRemoved(object sender, IEnumerable<MusicFileDTO> e)
        {
        }
    }
}