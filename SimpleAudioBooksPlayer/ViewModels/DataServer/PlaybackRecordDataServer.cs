using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Service;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class PlaybackRecordDataServer : IDataServer<PlaybackRecordDTO, PlaybackRecordDTO>
    {
        public static readonly PlaybackRecordDataServer Current = new PlaybackRecordDataServer();

        private readonly PlaybackRecordDataService _service;
        private readonly FileGroupDataServer _groupDataServer = FileGroupDataServer.Current;

        private PlaybackRecordDataServer()
        {
            _service = PlaybackRecordDataService.Current;
            Data = new ObservableCollection<PlaybackRecordDTO>();
        }

        public bool IsInit { get; private set; }
        public ObservableCollection<PlaybackRecordDTO> Data { get; }

        public event EventHandler<IEnumerable<PlaybackRecordDTO>> DataLoaded;
        public event EventHandler<IEnumerable<PlaybackRecordDTO>> DataAdded;
        public event EventHandler<IEnumerable<PlaybackRecordDTO>> DataRemoved;

        public async Task Init()
        {
            if (IsInit)
                return;

            IsInit = true;

            var data = await _service.GetData();
            foreach (var record in data)
                Data.Add(new PlaybackRecordDTO(record));

            DataLoaded?.Invoke(this, Data.ToList());

            _groupDataServer.DataRemoved += GroupDataServer_DataRemoved;
        }

        public async Task SetRecord(PlaybackRecordDTO record)
        {
            var pr = Data.FirstOrDefault(r => r.Group.Index == record.Group.Index);
            if (pr is null)
            {
                await _service.Add(record.ToTableObject());
                Data.Add(record);
                DataAdded?.Invoke(this, new[] {record});
            }
            else
            {
                var id = Data.IndexOf(pr);
                await _service.Update(record.ToTableObject());
                Data[id] = record;
            }
        }

        private async void GroupDataServer_DataRemoved(object sender, IEnumerable<FileGroupDTO> e)
        {
            var list = Data.Where(r => e.Any(g => g.Index == r.Group.Index)).ToList();
            await _service.RemoveRange(list.Select(r => r.Group.Index));
            foreach (var recordDto in list)
                Data.Remove(recordDto);

            DataRemoved?.Invoke(this, list);
        }
    }
}