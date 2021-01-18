using System;
using SimpleAudioBooksPlayer.Models.DTO;
using SimpleAudioBooksPlayer.Models.FileModels;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class PlayerNotifier
    {
        public static event EventHandler<MusicFile> CurrentItemChanged;
        public static event EventHandler<PlayerPositionChangeEventArgs> PositionChanged;
        public static event EventHandler<TimeSpan> PositionChangeRequested;

        public static void RaiseItem(MusicFile item)
        {
            CurrentItemChanged?.Invoke(null, item);
        }

        public static void RaisePosition(PlayerPositionChangeEventArgs args)
        {
            PositionChanged?.Invoke(null, args);
        }

        public static void RequestChangePosition(TimeSpan position)
        {
            PositionChangeRequested?.Invoke(null, position);
        }
    }
}