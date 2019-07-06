using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using GalaSoft.MvvmLight;

namespace SimpleAudioBooksPlayer.ViewModels.SidePages
{
    public class SettingsViewModel : ViewModelBase
    {
        private StorageLibrary _musicLibrary;

        public StorageLibrary MusicLibrary
        {
            get => _musicLibrary;
            set => Set(ref _musicLibrary, value);
        }
    }
}