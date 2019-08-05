using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class PlaybackListViewModel : ViewModelBase
    {
        public ObservableCollection<MusicFileDTO> PlaybackListSource { get; } = PlaybackListDataServer.Current.Data;

        public MusicFileDTO CurrentMusic => PlaybackListDataServer.Current.CurrentMusic;
    }
}