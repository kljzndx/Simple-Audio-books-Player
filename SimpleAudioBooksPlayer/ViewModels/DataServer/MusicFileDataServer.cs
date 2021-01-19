
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
        private FileScanner _scanner = new FileScanner("mp3", "aac", "flac", "alac", "m4a", "wav");
        private MusicFileFactory _factory = new MusicFileFactory();

        public MusicFileDataServer()
        {
            _scanner.Options.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
        }

        public ObservableCollection<MusicFile> Data { get; } = new ObservableCollection<MusicFile>();

        public async Task RefreshData(int groupId)
        {
            if (groupId == _groupId)
                return;

            this.LogByObject("加载并排序数据");
            _groupId = groupId;
            Data.Clear();

            if (PlaybackListDataServer.Current.CurrentGroup != null && groupId == PlaybackListDataServer.Current.CurrentGroup.Index)
            {
                foreach (var item in PlaybackListDataServer.Current.Data)
                    Data.Add(item);

                return;
            }
            
            var group = FileGroupDataServer.Current.Data.First(g => g.Index == groupId);

            await _scanner.ScanByFileQuery(await StorageFolder.GetFolderFromPathAsync(group.FolderPath), async files =>
            {
                foreach (var file in files)
                    Data.Add(await _factory.CreateByFile(file, group));
            });

            SortData(_settings.SortMethod);
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