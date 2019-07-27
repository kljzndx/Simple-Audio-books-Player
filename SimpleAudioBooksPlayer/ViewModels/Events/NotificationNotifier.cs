using System;
using SimpleAudioBooksPlayer.Log;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class NotificationNotifier
    {
        public static event EventHandler<string> ShowRequested;
        public static event EventHandler<object> HideRequested;

        public static void RequestShow(string text)
        {
            typeof(NotificationNotifier).LogByType(text);
            ShowRequested?.Invoke(null, text);
        }

        public static void RequestHide()
        {
            HideRequested?.Invoke(null, null);
        }
    }
}