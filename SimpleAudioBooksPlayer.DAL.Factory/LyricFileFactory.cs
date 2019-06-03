using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class LyricFileFactory : IFileFactory
    {
        public async Task<ILibraryFile> CreateByFile(StorageFile file, int dbVersion)
        {
            var basicProp = await file.GetBasicPropertiesAsync();
            return new LyricFile(file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<ILibraryFile> CreateByPath(string path, int dbVersion)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion);
        }
    }
}