using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;
using Nito.AsyncEx;

namespace SimpleAudioBooksPlayer.Models.FileFactories
{
    public static class MusicFileScanner
    {
        private static FileScanner _scanner = new FileScanner("mp3", "aac", "flac", "alac", "m4a", "wav");
        private static MusicFileFactory _factory = new MusicFileFactory();

        private static FileGroupDTO _groupDto;
        private static readonly List<MusicFile> _fileCaches = new List<MusicFile>();
        private static readonly AsyncLock _mutex = new AsyncLock();

        static MusicFileScanner()
        {
            _scanner.Options.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
        }
        
        public static async Task Scan(FileGroupDTO groupDTO, Func<IList<MusicFile>> data)
        {
            using (await _mutex.LockAsync())
            {
                var list = data();

                if (groupDTO.Equals(_groupDto))
                {
                    foreach (var mf in _fileCaches)
                        list.Add(mf);

                    return;
                }

                _fileCaches.Clear();
                _groupDto = groupDTO;

                await _scanner.ScanByFileQuery(await StorageFolder.GetFolderFromPathAsync(groupDTO.FolderPath), async files =>
                {
                    foreach (var file in files)
                    {
                        var mf = await _factory.CreateByFile(file, groupDTO);
                        list.Add(mf);
                        _fileCaches.Add(mf);
                    }
                });
            }
        }
    }
}