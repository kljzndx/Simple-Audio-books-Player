using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class PlayRecordViewModel : ViewModelBase
    {
        private readonly PlaybackRecordDataServer _dataServer = PlaybackRecordDataServer.Current;

        public ObservableCollection<PlaybackRecordDTO> Data => _dataServer.Data;
    }
}