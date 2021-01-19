using System;
using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Log;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;

namespace SimpleAudioBooksPlayer.Models.FileFactories
{
    public class MusicFileFactory : IFileFactory<MusicFile>
    {
        public async Task<MusicFile> CreateByFile(StorageFile file, FileGroupDTO group)
        {
            this.LogByObject("获取属性");
            var basicProp = await file.GetBasicPropertiesAsync();
            var musicProp = await file.Properties.GetMusicPropertiesAsync();
            
            this.LogByObject("创建文件对象");
            return new MusicFile(group, file.Name, basicProp.DateModified.DateTime, musicProp.TrackNumber, musicProp.Title, musicProp.Duration);
        }

        public async Task<MusicFile> CreateByPath(string path, FileGroupDTO group)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), group);
        }
    }
}