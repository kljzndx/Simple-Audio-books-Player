using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;
using Nito.AsyncEx;
using SimpleAudioBooksPlayer.ViewModels.Events;
using Windows.ApplicationModel.Resources;
using System.Linq;

namespace SimpleAudioBooksPlayer.Models.FileFactories
{
    public static class FileDataScanner
    {
        public const string MusicFileExtensionNames = "mp3 aac flac alac m4a wav";
        public const string SubtitleFileExtensionNames = "lrc srt";
        
        private static FileGroupDTO _groupDto;
        private static readonly Dictionary<Type, IEnumerable<IFile>> _fileCaches = new Dictionary<Type, IEnumerable<IFile>>();
        private static readonly AsyncLock _mutex = new AsyncLock();

        private static readonly string _musicNotificationString;
        private static readonly string _subtitleNotificationString;

        static FileDataScanner()
        {
            _musicNotificationString = ResourceLoader.GetForCurrentView("Notifications").GetString("ScanningMusicFiles");
            _subtitleNotificationString = ResourceLoader.GetForCurrentView("Notifications").GetString("ScanningSubtitleFiles");
        }

        public static async Task<List<MusicFile>> ScanMusicData(FileGroupDTO groupDTO)
        {
            NotificationNotifier.RequestShow(_musicNotificationString.Replace("$$", groupDTO.Name));

            var data = await Scan(groupDTO, new MusicFileFactory(), MusicFileExtensionNames);
            
            NotificationNotifier.RequestHide();
            return data;
        }

        public static async Task<List<SubtitleFile>> ScanSubtitleData(FileGroupDTO groupDTO)
        {
            NotificationNotifier.RequestShow(_subtitleNotificationString.Replace("$$", groupDTO.Name));

            var data = await Scan(groupDTO, new SubtitleFileFactory(), SubtitleFileExtensionNames);
            
            NotificationNotifier.RequestHide();
            return data;
        }

        private static async Task<List<TFile>> Scan<TFile>(FileGroupDTO groupDTO, IFileFactory<TFile> fileFactory, string extensionNames) where TFile: class, IFile
        {
            using (await _mutex.LockAsync())
            {
                if (groupDTO.Equals(_groupDto))
                {
                    if (_fileCaches.ContainsKey(typeof(TFile)))
                        return (List<TFile>)_fileCaches[typeof(TFile)];
                }
                else
                {
                    _groupDto = groupDTO;
                    _fileCaches.Clear();
                }

                var list = new List<TFile>();
                
                var scanner = new FileScanner(extensionNames.Split(" "));
                scanner.Options.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
                
                await scanner.ScanByFileQuery(await StorageFolder.GetFolderFromPathAsync(groupDTO.FolderPath), async files =>
                {
                    foreach (var file in files)
                    {
                        var data = await fileFactory.CreateByFile(file, groupDTO);
                        list.Add(data);
                    }
                });

                return list;
            }
        }
    }
}