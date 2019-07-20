﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class ClassListDataServer : IDataServer<ClassItemDTO, ClassItemDTO>
    {
        public static readonly ClassListDataServer Current = new ClassListDataServer();

        public static ClassItemDTO Unspecified_ClassItem { get; private set; }
        public static ClassItemDTO All_ClassItem { get; private set; }

        private readonly ClassListDataService _service = ClassListDataService.Current;

        private ClassListDataServer()
        {
            _service.DataAdded += Service_DataAdded;
        }

        public event EventHandler<IEnumerable<ClassItemDTO>> DataLoaded;
        public event EventHandler<IEnumerable<ClassItemDTO>> DataAdded;
        public event EventHandler<IEnumerable<ClassItemDTO>> DataRemoved;
        public event EventHandler<IEnumerable<ClassItemDTO>> DataUpdated;

        public bool IsInit { get; private set; }
        public ObservableCollection<ClassItemDTO> Data { get; } = new ObservableCollection<ClassItemDTO>();

        public async Task Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            var dataSource = await _service.GetData();
            foreach (var item in dataSource)
                Data.Add(new ClassItemDTO(item));

            if (Data.All(c => c.Index != -1))
                await _service.Add(-1, StringResources.DefaultValues.GetString("Unspecified_ClassItem"));

            if (Data.All(c => c.Index != 1))
                await Add(StringResources.DefaultValues.GetString("All_ClassItem"));

            Unspecified_ClassItem = Data.First(c => c.Index == -1);
            All_ClassItem = Data.First(c => c.Index == 1);

            DataLoaded?.Invoke(this, Data.ToList());
        }

        public async Task Rename(ClassItemDTO itemDto, string newName)
        {
            itemDto.Name = newName;
            await _service.Rename(itemDto.Index, newName);
            DataUpdated?.Invoke(this, new[] {itemDto});
        }

        public async Task Remove(ClassItemDTO itemDto)
        {
            if (!Data.Contains(itemDto))
                return;

            Data.Remove(itemDto);
            await _service.Remove(itemDto.Index);

            DataRemoved?.Invoke(this, new[] {itemDto});
        }

        // 因为要获取id号，所以只能用回调来添加项目
        public Task Add(string name) => _service.Add(name);

        private void Service_DataAdded(object sender, IEnumerable<ClassItem> e)
        {
            var source = e.Select(c => new ClassItemDTO(c)).ToList();
            foreach (var dto in source)
                if (Data.Any() && dto.Index == -1)
                    Data.Insert(0, dto);
                else
                    Data.Add(dto);

            DataAdded?.Invoke(this, source);
        }

    }
}