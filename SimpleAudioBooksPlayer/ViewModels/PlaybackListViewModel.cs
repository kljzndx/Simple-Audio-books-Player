using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HappyStudio.Subtitle.Control.UWP.Models;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class PlaybackListViewModel : ViewModelBase
    {
        private readonly SubtitleFileDataServer _subtitleFileServer = SubtitleFileDataServer.Current;
        
        public ObservableCollection<MusicFileDTO> PlaybackListSource { get; } = PlaybackListDataServer.Current.Data;

        public MusicFileDTO CurrentMusic => PlaybackListDataServer.Current.CurrentMusic;

        public async Task<List<SubtitleLineUi>> GetSubtitleLines()
        {
            if (CurrentMusic is null)
                return null;

            var file = _subtitleFileServer.Data.FirstOrDefault(s => s.DisplayName == CurrentMusic.DisplayName);
            if (file is null)
                return null;

            var subtitle = await file.GetLines();
            return subtitle;
        }
    }
}