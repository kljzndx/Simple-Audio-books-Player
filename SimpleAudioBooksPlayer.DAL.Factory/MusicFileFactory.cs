using System;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Storage;

namespace SimpleAudioBooksPlayer.DAL.Factory
{
    public class MusicFileFactory : IFileFactory<MusicFile>
    {
        public async Task<MusicFile> CreateByFile(StorageFile file, int dbVersion)
        {
            var basicProp = await file.GetBasicPropertiesAsync();
            var musicProp = await file.Properties.GetMusicPropertiesAsync();
            return new MusicFile(musicProp.Title, musicProp.Duration, file.Path, basicProp.DateModified.DateTime, dbVersion);
        }

        public async Task<MusicFile> CreateByPath(string path, int dbVersion)
        {
            return await CreateByFile(await StorageFile.GetFileFromPathAsync(path), dbVersion);
        }
    }
}