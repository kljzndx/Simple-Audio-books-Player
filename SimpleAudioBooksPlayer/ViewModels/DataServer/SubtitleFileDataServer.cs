
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileFactories;
using SimpleAudioBooksPlayer.Models.FileModels;

namespace SimpleAudioBooksPlayer.ViewModels.DataServer
{
    public class SubtitleFileDataServer
    {
        public static readonly SubtitleFileDataServer Current = new SubtitleFileDataServer();

        private FileScanner _scanner = new FileScanner("lrc", "srt");
        private SubtitleFileFactory _factory = new SubtitleFileFactory();

        public SubtitleFileDataServer()
        {
            _scanner.Options.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
        }

        public ObservableCollection<SubtitleFile> Data { get; } = new ObservableCollection<SubtitleFile>();

        public async Task Scan(FileGroupDTO group)
        {
            Data.Clear();

            await _scanner.ScanByFileQuery(await StorageFolder.GetFolderFromPathAsync(group.FolderPath), async files =>
            {
                foreach (var file in files)
                    Data.Add(await _factory.CreateByFile(file, group));
            });
        }
    }
}