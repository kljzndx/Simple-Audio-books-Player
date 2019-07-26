using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.DAL.Factory;
using SimpleAudioBooksPlayer.Log;

namespace SimpleAudioBooksPlayer.Service
{
    public class MusicLibraryDataServiceManager
    {
        private const string MusicExtensionNames = "mp3 aac flac alac m4a wav";
        private const string LyricExtensionNames = "lrc";

        public static readonly MusicLibraryDataServiceManager Current;

        private MusicLibraryDataService<MusicFile, MusicFileFactory> _musicService;
        private MusicLibraryDataService<SubtitleFile, SubtitleFileFactory> _lyricService;

        static MusicLibraryDataServiceManager()
        {
            Current = new MusicLibraryDataServiceManager();
        }

        public async Task<MusicLibraryDataService<MusicFile, MusicFileFactory>> GetMusicService()
        {
            if (_musicService is null)
                _musicService = await MusicLibraryDataService<MusicFile, MusicFileFactory>
                    .GetService(MusicExtensionNames.Split(' '));

            return _musicService;
        }

        public async Task<MusicLibraryDataService<SubtitleFile, SubtitleFileFactory>> GetLyricService()
        {
            if (_lyricService is null)
                _lyricService = await MusicLibraryDataService<SubtitleFile, SubtitleFileFactory>.GetService(LyricExtensionNames);

            return _lyricService;
        }

        public async Task ScanFiles()
        {
            this.LogByObject("获取服务");
            var musicService = await GetMusicService();
            var lyricService = await GetLyricService();

            this.LogByObject("开始扫描");
            await musicService.ScanFiles();
            await lyricService.ScanFiles();
            this.LogByObject("扫描完成");
        }
    }
}