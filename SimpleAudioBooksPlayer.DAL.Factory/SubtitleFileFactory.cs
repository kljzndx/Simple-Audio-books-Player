using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class SubtitleFileFactory : IFileFactory<SubtitleFile>
    {
        public async Task<SubtitleFile> CreateByFile(StorageFile file, int dbVersion)
        {
            var basicProp = await file.GetBasicPropertiesAsync();
            return new SubtitleFile(file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<SubtitleFile> CreateByPath(string path, int dbVersion)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion);
        }
    }
}