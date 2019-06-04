using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class LyricFileFactory : IFileFactory<LyricFile>
    {
        public async Task<LyricFile> CreateByFile(StorageFile file, int dbVersion)
        {
            var basicProp = await file.GetBasicPropertiesAsync();
            return new LyricFile(file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<LyricFile> CreateByPath(string path, int dbVersion)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion);
        }
    }
}