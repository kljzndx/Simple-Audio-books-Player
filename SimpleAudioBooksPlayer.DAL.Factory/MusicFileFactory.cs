using System;
using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Log;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class MusicFileFactory : IFileFactory<MusicFile>
    {
        public async Task<MusicFile> CreateByFile(StorageFile file, int dbVersion, FileGroup group)
        {
            this.LogByObject("获取属性");
            var basicProp = await file.GetBasicPropertiesAsync();
            var musicProp = await file.Properties.GetMusicPropertiesAsync();
            this.LogByObject("创建文件对象");
            return new MusicFile(group, musicProp.TrackNumber, musicProp.Title, musicProp.Duration, file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<MusicFile> CreateByPath(string path, int dbVersion, FileGroup group)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion, group);
        }
    }
}