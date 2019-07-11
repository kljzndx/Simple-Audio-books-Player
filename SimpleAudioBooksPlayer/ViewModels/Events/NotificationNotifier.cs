using System;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class NotificationNotifier
    {
        public static event EventHandler<string> ShowRequested;
        public static event EventHandler<object> HideRequested;

        public static void RequestShow(string text)
        {
            ShowRequested?.Invoke(null, text);
        }

        public static void RequestHide()
        {
            HideRequested?.Invoke(null, null);
        }
    }
}