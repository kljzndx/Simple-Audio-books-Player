using System.Threading.Tasks;
using SimpleAudioBooksPlayer.DAL;
using SimpleAudioBooksPlayer.DAL.Factory;
using SimpleAudioBooksPlayer.Log;

namespace SimpleAudioBooksPlayer.Service
{
    public class MusicLibraryDataServiceManager
    {
        private const string MusicExtensionNames = "mp3 aac flac alac m4a wav";
        private const string SubtitleExtensionNames = "lrc srt";

        public static readonly MusicLibraryDataServiceManager Current;

        private MusicLibraryDataService<MusicFile, MusicFileFactory> _musicService;
        private MusicLibraryDataService<SubtitleFile, SubtitleFileFactory> _subtitleService;

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
            if (_subtitleService is null)
                _subtitleService =
                    await MusicLibraryDataService<SubtitleFile, SubtitleFileFactory>.GetService(
                        SubtitleExtensionNames.Split(' '));

            return _subtitleService;
        }

        public async Task ScanFiles()
        {
            this.LogByObject("获取服务");
            var musicService = await GetMusicService();
            var subtitleService = await GetLyricService();

            this.LogByObject("开始扫描");
            await musicService.ScanFiles();
            await subtitleService.ScanFiles();
            this.LogByObject("扫描完成");
        }
    }
}