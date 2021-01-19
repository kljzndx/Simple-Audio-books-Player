using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using HappyStudio.UwpToolsLibrary.Auxiliarys.Files.Scanners;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;

namespace SimpleAudioBooksPlayer.Models.FileFactories
{
    public static class MusicFileScanner
    {
        private static FileScanner _scanner = new FileScanner("mp3", "aac", "flac", "alac", "m4a", "wav");
        private static MusicFileFactory _factory = new MusicFileFactory();

        static MusicFileScanner()
        {
            _scanner.Options.FolderDepth = Windows.Storage.Search.FolderDepth.Shallow;
        }
        
        public static async Task Scan(FileGroupDTO groupDTO, Func<IList<MusicFile>> data)
        {
            // TODO 异步队列

            var list = data();
            
            await _scanner.ScanByFileQuery(await StorageFolder.GetFolderFromPathAsync(groupDTO.FolderPath), async files =>
            {
                foreach (var file in files)
                {
                    list.Add(await _factory.CreateByFile(file, groupDTO));
                }
            });
        }
    }
}