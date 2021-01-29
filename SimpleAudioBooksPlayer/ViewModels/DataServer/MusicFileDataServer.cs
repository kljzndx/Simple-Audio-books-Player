using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileFactories;
using SimpleAudioBooksPlayer.Models.FileModels;
using SimpleAudioBooksPlayer.Models.Sorters;
using SimpleAudioBooksPlayer.ViewModels.SettingProperties;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class MusicFileDataServer
    {
        public static readonly MusicFileDataServer Current = new MusicFileDataServer();

        private readonly MusicListSettingProperties _settings = MusicListSettingProperties.Current;
        private int _groupId;

        public ObservableCollection<MusicFile> Data { get; } = new ObservableCollection<MusicFile>();

        public FileGroupDTO CurrentGroup { get; private set; }

        public async Task RefreshData(int groupId)
        {
            if (groupId == _groupId)
                return;

            this.LogByObject("加载并排序数据");
            _groupId = groupId;
            Data.Clear();
            FileGroupDTO groupDto;

            if (PlaybackListDataServer.Current.CurrentGroup != null && groupId == PlaybackListDataServer.Current.CurrentGroup.Index)
            {
                groupDto = PlaybackListDataServer.Current.CurrentGroup;
                foreach (var item in PlaybackListDataServer.Current.Data)
                    Data.Add(item);
            }
            else
            {
                groupDto = FileGroupDataServer.Current.Data.First(g => g.Index == groupId);

                var fds = await FileDataScanner.ScanMusicData(groupDto);
                foreach (var musicFile in fds)
                    Data.Add(musicFile);
            }
            
            SortData(_settings.SortMethod);
            CurrentGroup = groupDto;
        }

        public void SortData(MusicListSortMembers method)
        {
            this.LogByObject($"按 {method} 排序数据");
            _settings.SortMethod = method;

            IEnumerable<MusicFile> source = Data.OrderBy(MusicSortDeserialization.Deserialize(method).Invoke);

            if (_settings.IsReverse)
                source = source.Reverse();

            var data = source.ToList();

            for (var i = 0; i < data.Count; i++)
                Data.Move(Data.IndexOf(data[i]), i);
        }

        public void Reverse()
        {
            this.LogByObject("倒序排序数据");
            _settings.IsReverse = !_settings.IsReverse;

            var data = Data.Reverse().ToList();
            Data.Clear();
            foreach (var file in data)
                Data.Add(file);
        }
    }
}