using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using SimpleAudioBooksPlayer.Log;
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

            this.LogByObject("初始化数据");
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
                this.LogByObject("添加播放记录");
                await _service.Add(record.ToTableObject());
                Data.Insert(0, record);
                DataAdded?.Invoke(this, new[] {record});
            }
            else
            {
                this.LogByObject("更新播放记录");
                await _service.Update(record.ToTableObject());
                pr.Update(record);

                int id = Data.IndexOf(pr);
                if (id != 0)
                    Data.Move(id, 0);
            }
        }

        private async void GroupDataServer_DataRemoved(object sender, IEnumerable<FileGroupDTO> e)
        {
            var list = Data.Where(r => e.Any(g => g.Index == r.Group.Index)).ToList();
            if (!list.Any())
                return;

            this.LogByObject("移除无效播放记录");
            await _service.RemoveRange(list.Select(r => r.Group.Index));
            foreach (var recordDto in list)
                Data.Remove(recordDto);

            DataRemoved?.Invoke(this, list);
        }
    }
}