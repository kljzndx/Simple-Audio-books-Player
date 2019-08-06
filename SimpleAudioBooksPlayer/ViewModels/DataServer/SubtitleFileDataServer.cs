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
    public class SubtitleFileDataServer : IFileDataServer<SubtitleFileDTO>
    {
        public static readonly SubtitleFileDataServer Current = new SubtitleFileDataServer();

        private IObservableDataService<SubtitleFile> _service;

        private SubtitleFileDataServer()
        {
            Data = new ObservableCollection<SubtitleFileDTO>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<SubtitleFileDTO> Data { get; }

        public event EventHandler<IEnumerable<SubtitleFileDTO>> DataLoaded;
        public event EventHandler<IEnumerable<SubtitleFileDTO>> DataAdded;
        public event EventHandler<IEnumerable<SubtitleFileDTO>> DataRemoved;
        public event EventHandler<IEnumerable<SubtitleFileDTO>> DataUpdated;

        public async Task Init()
        {
            if (IsInit)
                return;
            IsInit = true;

            _service = await MusicLibraryDataServiceManager.Current.GetLyricService();
            var data = await _service.GetData();

            foreach (var subtitleFile in data)
                Data.Add(new SubtitleFileDTO(subtitleFile));

            DataLoaded?.Invoke(this, Data.ToList());

            _service.DataAdded += Service_DataAdded;
            _service.DataRemoved += Service_DataRemoved;
            _service.DataUpdated += Service_DataUpdated;
        }

        private void Service_DataAdded(object sender, IEnumerable<SubtitleFile> e)
        {
            var list = e.Select(s => new SubtitleFileDTO(s)).ToList();
            foreach (var file in list)
                Data.Add(file);

            DataAdded?.Invoke(this, list);
        }

        private void Service_DataRemoved(object sender, IEnumerable<SubtitleFile> e)
        {
            var list = Data.Where(src => e.Any(s => s.FilePath == src.FilePath)).ToList();
            foreach (var fileDto in list)
                Data.Remove(fileDto);

            DataRemoved?.Invoke(this, list);
        }

        private void Service_DataUpdated(object sender, IEnumerable<SubtitleFile> e)
        {
            var args = e.ToList();
            var list = Data.Where(src => args.Any(s => s.FilePath == src.FilePath)).ToList();
            foreach (var fileDto in list)
                fileDto.Update(args.First(s => s.FilePath == fileDto.FilePath));

            DataUpdated?.Invoke(this, list);
        }
    }
}