using System;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class BuyToolNotifier
    {
        public static event EventHandler BuyRequested;

        public static void RequestBuy()
        {
            BuyRequested?.Invoke(null, EventArgs.Empty);
        }
    }
}