using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class FileGroupDataServer : IDataServer<FileGroupDTO, FileGroupDTO>
    {
        public static readonly FileGroupDataServer Current = new FileGroupDataServer();

        private FileGroupDataService _service;
        private StorageFolder _coverFolder;

        public FileGroupDataServer()
        {
            Data = new ObservableCollection<FileGroupDTO>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<FileGroupDTO> Data { get; }

        public event EventHandler<IEnumerable<FileGroupDTO>> DataLoaded;
        public event EventHandler<IEnumerable<FileGroupDTO>> DataAdded;
        public event EventHandler<IEnumerable<FileGroupDTO>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;
            _service = FileGroupDataService.Current;

            var source = await _service.GetData();
            var data = source.Select(g => new FileGroupDTO(g)).ToList();
            foreach (var item in data)
                Data.Add(item);

            DataLoaded?.Invoke(this, data);
            _service.DataAdded += Service_DataAdded;
            _service.DataRemoved += Service_DataRemoved;
            _service.DataUpdated += Service_DataUpdated;
        }

        public async Task SetCover(FileGroupDTO groupDto, StorageFile file)
        {
            if (_coverFolder is null)
                if (!OtherSettingProperties.Current.IsCreatedCoverFolder)
                {
                    _coverFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("cover");
                    OtherSettingProperties.Current.IsCreatedCoverFolder = true;
                }
                else
                    _coverFolder = await ApplicationData.Current.LocalFolder.GetFolderAsync("cover");

            var bitmapFile = await file.CopyAsync(_coverFolder, $"{groupDto.Index}.image", NameCollisionOption.ReplaceExisting);
            var bi = new BitmapImage();
            bi.SetSource(await bitmapFile.OpenAsync(FileAccessMode.Read));
            groupDto.SetCover(bi);
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
            foreach (var fileGroup in list)
                Data.First(g => g.Index == fileGroup.Index).Update(fileGroup);
        }
    }
}