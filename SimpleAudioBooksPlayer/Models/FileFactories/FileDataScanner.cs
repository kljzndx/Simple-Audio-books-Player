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
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files;
using Newtonsoft.Json;
using SimpleAudioBooksPlayer.Log;

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

        public static async Task<List<MusicFile>> ScanMusicData(FileGroupDTO groupDTO, bool isForceScan = false)
        {
            NotificationNotifier.RequestShow(_musicNotificationString.Replace("$$", groupDTO.Name));

            var data = await Scan(groupDTO, new MusicFileFactory(), MusicFileExtensionNames, "music-files.json", isForceScan);
            
            NotificationNotifier.RequestHide();
            return data;
        }

        public static async Task<List<SubtitleFile>> ScanSubtitleData(FileGroupDTO groupDTO, bool isForceScan = false)
        {
            NotificationNotifier.RequestShow(_subtitleNotificationString.Replace("$$", groupDTO.Name));

            var data = await Scan(groupDTO, new SubtitleFileFactory(), SubtitleFileExtensionNames, "subtitle-files.json", isForceScan);
            
            NotificationNotifier.RequestHide();
            return data;
        }

        private static async Task<List<TFile>> Scan<TFile>(FileGroupDTO groupDTO, IFileFactory<TFile> fileFactory, string extensionNames, string cacheFileName, bool isForceScan) where TFile: LibraryFileBase
        {
            using (await _mutex.LockAsync())
            {
                if (!groupDTO.Equals(_groupDto))
                {
                    typeof(FileDataScanner).LogByType("清空缓存");
                    _groupDto = groupDTO;
                    _fileCaches.Clear();
                }

                var list = new List<TFile>();

                if (isForceScan)
                {
                    if (groupDTO.Equals(_groupDto) && _fileCaches.ContainsKey(typeof(TFile)))
                    {
                        typeof(FileDataScanner).LogByType("删除对应的缓存");
                        _fileCaches.Remove(typeof(TFile));
                    }
                }
                else
                {
                    if (groupDTO.Equals(_groupDto) && _fileCaches.ContainsKey(typeof(TFile)))
                    {
                        typeof(FileDataScanner).LogByType("直接返回缓存");
                        return (List<TFile>)_fileCaches[typeof(TFile)];
                    }

                    typeof(FileDataScanner).LogByType("查找索引文件");
                    if (await (await groupDTO.GetInfoFolder()).TryGetItemAsync(cacheFileName) is StorageFile cf)
                    {
                        typeof(FileDataScanner).LogByType("读取索引文件");
                        var content = await FileReader.ReadText(cf, "GBK");
                        list = JsonConvert.DeserializeObject<List<TFile>>(content);
                        
                        foreach (var file in list)
                            file.Group = groupDTO;
                        
                        return list;
                    }
                }

                typeof(FileDataScanner).LogByType("扫描文件");
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

                typeof(FileDataScanner).LogByType("添加缓存");
                _fileCaches.Add(typeof(TFile), list);

                if (list.Any())
                {
                    typeof(FileDataScanner).LogByType("写入索引文件");
                    var infoFolder = await groupDTO.GetInfoFolder();
                    var cacheFile = await infoFolder.CreateFileAsync(cacheFileName, CreationCollisionOption.OpenIfExists);
                    await FileIO.WriteTextAsync(cacheFile, JsonConvert.SerializeObject(list));
                }

                return list;
            }
        }
    }
}