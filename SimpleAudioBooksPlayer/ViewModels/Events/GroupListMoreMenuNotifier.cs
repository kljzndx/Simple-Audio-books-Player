using Windows.Foundation;
using Windows.UI.Xaml;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class GroupListMoreMenuNotifier
    {
        public static event TypedEventHandler<FrameworkElement, object> ShowMoreMenuRequested;

        public static void RequestShowMoreMenu(FrameworkElement element)
        {
            ShowMoreMenuRequested?.Invoke(element, null);
        }
    }
}