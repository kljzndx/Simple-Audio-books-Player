﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using HappyStudio.Parsing.Subtitle.Interfaces;
using SimpleAudioBooksPlayer.Models.FileModels;
using SimpleAudioBooksPlayer.ViewModels.DataServer;

namespace SimpleAudioBooksPlayer.ViewModels
{
    public class PlaybackListViewModel : ViewModelBase
    {
        private readonly SubtitleFileDataServer _subtitleFileServer = SubtitleFileDataServer.Current;
        
        public ObservableCollection<MusicFile> PlaybackListSource { get; } = PlaybackListDataServer.Current.Data;

        public MusicFile CurrentMusic => PlaybackListDataServer.Current.CurrentMusic;

        public async Task<List<ISubtitleLine>> GetSubtitleLines(bool isForceScan = false)
        {
            if (CurrentMusic is null)
                return null;

            await SubtitleFileDataServer.Current.Scan(CurrentMusic.Group, isForceScan);
            var file = _subtitleFileServer.Data.FirstOrDefault(s => s.DisplayName == CurrentMusic.DisplayName);
            if (file is null)
                return null;

            var subtitle = await file.GetSubtitleLines();
            if (subtitle == null)
            {
                await SubtitleFileDataServer.Current.RemoveItem(file);
                return await GetSubtitleLines(false);
            }

            return subtitle;
        }
    }
}