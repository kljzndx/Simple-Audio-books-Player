using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.DAL.Factory;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class MusicFileDataServer : IFileDataServer<MusicFileDTO>
    {
        public static readonly MusicFileDataServer Current = new MusicFileDataServer();

        private IObservableDataService<MusicFile> _service;

        public bool IsInit { get; private set; }
        public ObservableCollection<MusicFileDTO> Data { get; } = new ObservableCollection<MusicFileDTO>();

        public event EventHandler<IEnumerable<MusicFileDTO>> DataLoaded;
        public event EventHandler<IEnumerable<MusicFileDTO>> DataAdded;
        public event EventHandler<IEnumerable<MusicFileDTO>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;
            _service = await MusicLibraryDataServiceManager.Current.GetMusicService();

            var source = await _service.GetData();
            var data = source.Select(f => new MusicFileDTO(f)).ToList();
            foreach (var fileDto in data)
                Data.Add(fileDto);

            DataLoaded?.Invoke(this, data);
            _service.DataAdded += Service_DataAdded;
            _service.DataRemoved += Service_DataRemoved;
            _service.DataUpdated += Service_DataUpdated;
        }

        private void Service_DataAdded(object sender, IEnumerable<MusicFile> e)
        {
            var mfdList = e.Select(f => new MusicFileDTO(f)).ToList();
            foreach (var mfd in mfdList)
                Data.Add(mfd);

            DataAdded?.Invoke(this, mfdList);
        }

        private void Service_DataRemoved(object sender, IEnumerable<MusicFile> e)
        {
            var needRemove = Data.Where(d => e.Any(f => f.FilePath == d.FilePath)).ToList();
            foreach (var fileDto in needRemove)
                Data.Remove(fileDto);

            DataRemoved?.Invoke(this, needRemove);
        }

        private void Service_DataUpdated(object sender, IEnumerable<MusicFile> e)
        {
            var needRemove = Data.Where(d => e.Any(f => f.FilePath == d.FilePath)).ToList();
            var needAdd = e.Select(f => new MusicFileDTO(f)).ToList();

            foreach (var fileDto in needRemove)
                Data.Remove(fileDto);

            foreach (var mfd in needAdd)
                Data.Add(mfd);

            DataRemoved?.Invoke(this, needRemove);
            DataAdded?.Invoke(this, needAdd);
        }
    }
}