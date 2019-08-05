using System;
using SimpleAudioBooksPlayer.Models.DTO;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class PlayerNotifier
    {
        public static event EventHandler<MusicFileDTO> CurrentItemChanged;
        public static event EventHandler<PlayerPositionChangeEventArgs> PositionChanged;
        public static event EventHandler<TimeSpan> PositionChangeRequested;

        public static void RaiseItem(MusicFileDTO item)
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