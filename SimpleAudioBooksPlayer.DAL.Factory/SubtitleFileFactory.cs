using System;
using System.Threading.Tasks;
using Windows.Storage;
using SimpleAudioBooksPlayer.Log;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class SubtitleFileFactory : IFileFactory<SubtitleFile>
    {
        public async Task<SubtitleFile> CreateByFile(StorageFile file, int dbVersion, FileGroup group)
        {
            this.LogByObject("获取属性");
            var basicProp = await file.GetBasicPropertiesAsync();
            this.LogByObject("创建文件对象");
            return new SubtitleFile(group, file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<SubtitleFile> CreateByPath(string path, int dbVersion, FileGroup group)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion, group);
        }
    }
}