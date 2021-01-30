using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileFactories;
using SimpleAudioBooksPlayer.Models.FileModels;
using Windows.ApplicationModel.Resources;
using Nito.AsyncEx;
using SimpleAudioBooksPlayer.ViewModels.Events;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class SubtitleFileDataServer
    {
        public static readonly SubtitleFileDataServer Current = new SubtitleFileDataServer();
        private FileGroupDTO _group;

        public SubtitleFileDataServer()
        {
        }

        public ObservableCollection<SubtitleFile> Data { get; } = new ObservableCollection<SubtitleFile>();

        public async Task Scan(FileGroupDTO group, bool isForceScan = false)
        {
            if (group.Equals(_group) && !isForceScan)
                return;

            _group = group;
            Data.Clear();
            
            var fds = await FileDataScanner.ScanSubtitleData(group, isForceScan);
            foreach (var subtitleFile in fds)
                Data.Add(subtitleFile);
        }
    }
}