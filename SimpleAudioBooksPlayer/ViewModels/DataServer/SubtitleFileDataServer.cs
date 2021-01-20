
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

        private FileScanner _scanner = new FileScanner("lrc", "srt");
        private SubtitleFileFactory _factory = new SubtitleFileFactory();
        private readonly string _notificationString;
        private readonly AsyncLock _mutex = new AsyncLock();

        public SubtitleFileDataServer()
        {
            _scanner.Options.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
            _notificationString = ResourceLoader.GetForCurrentView("Notifications").GetString("ScanningSubtitleFiles");
        }

        public ObservableCollection<SubtitleFile> Data { get; } = new ObservableCollection<SubtitleFile>();

        public async Task Scan(FileGroupDTO group)
        {
            using (await _mutex.LockAsync())
            {
                Data.Clear();

                NotificationNotifier.RequestShow(_notificationString.Replace("$$", group.Name));

                await _scanner.ScanByFileQuery(await StorageFolder.GetFolderFromPathAsync(group.FolderPath), async files =>
                {
                    foreach (var file in files)
                        Data.Add(await _factory.CreateByFile(file, group));
                });

                NotificationNotifier.RequestHide();
            }
        }
    }
}