using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class ClassListDataServer : IDataServer<ClassItemDTO, ClassItemDTO>
    {
        public static readonly ClassListDataServer Current = new ClassListDataServer();

        private readonly ClassListDataService _service = ClassListDataService.Current;

        private ClassListDataServer()
        {
            
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

            if (!Data.Any())
                await _service.Add("All");

            DataLoaded?.Invoke(this, Data.ToList());

            _service.DataAdded += Service_DataAdded;
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

        public Task Add(string name) => _service.Add(name);

        private void Service_DataAdded(object sender, IEnumerable<ClassItem> e)
        {
            var source = e.Select(c => new ClassItemDTO(c)).ToList();
            foreach (var dto in source)
                Data.Add(dto);

            DataAdded?.Invoke(this, source);
        }

    }
}