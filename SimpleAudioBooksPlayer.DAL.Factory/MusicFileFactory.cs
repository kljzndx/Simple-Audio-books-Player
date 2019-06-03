using System;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class MusicFileFactory : IFileFactory
    {
        public async Task<ILibraryFile> CreateByFile(StorageFile file, int dbVersion)
        {
            var basicProp = await file.GetBasicPropertiesAsync();
            var musicProp = await file.Properties.GetMusicPropertiesAsync();
            return new MusicFile(musicProp.Title, musicProp.Duration, file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<ILibraryFile> CreateByPath(string path, int dbVersion)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion);
        }
    }
}