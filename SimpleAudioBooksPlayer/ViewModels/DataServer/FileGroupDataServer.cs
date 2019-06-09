﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class FileGroupDataServer : IDataServer<FileGroupDTO, FileGroupDTO>
    {
        public static readonly FileGroupDataServer Current = new FileGroupDataServer();

        private FileGroupDataService _service;

        public FileGroupDataServer()
        {
            Data = new ObservableCollection<FileGroupDTO>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<FileGroupDTO> Data { get; }
        public event EventHandler<IEnumerable<FileGroupDTO>> DataAdded;
        public event EventHandler<IEnumerable<FileGroupDTO>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;
            _service = FileGroupDataService.Current;

            var data = await _service.GetData();
            foreach (var item in data)
                Data.Add(new FileGroupDTO(item));

            _service.DataAdded += Service_DataAdded;
            _service.DataRemoved += Service_DataRemoved;
            _service.DataUpdated += Service_DataUpdated;
        }

        private void Service_DataAdded(object sender, IEnumerable<FileGroup> e)
        {
            var needAdd = e.Select(g => new FileGroupDTO(g)).ToList();
            foreach (var group in needAdd)
                Data.Add(group);

            DataAdded?.Invoke(this, needAdd);
        }

        private void Service_DataRemoved(object sender, IEnumerable<FileGroup> e)
        {
            var needRemove = new List<FileGroupDTO>(Data.Where(src => e.Any(g => g.Index == src.Index)));
            foreach (var groupDto in needRemove)
                Data.Remove(groupDto);

            DataRemoved?.Invoke(this, needRemove);
        }

        private void Service_DataUpdated(object sender, IEnumerable<FileGroup> e)
        {
            var list = e.ToList();
            var needRemove = new List<FileGroupDTO>(Data.Where(src => list.Any(g => g.Index == src.Index)));
            foreach (var groupDto in needRemove)
                Data.Remove(groupDto);

            var needAdd = list.Select(g => new FileGroupDTO(g)).ToList();
            foreach (var group in needAdd)
                Data.Add(group);

            DataRemoved?.Invoke(this, needRemove);
            DataAdded?.Invoke(this, needAdd);
        }
    }
}