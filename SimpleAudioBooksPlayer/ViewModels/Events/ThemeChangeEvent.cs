using System;
using Windows.UI.Xaml;

namespace SimpleAudioBooksPlayer.ViewModels.Events
{
    public static class ThemeChangeEvent
    {
        public static event EventHandler<ApplicationTheme> ThemeChanged;

        public static void ReportThemeChange(ApplicationTheme theme)
        {
            ThemeChanged?.Invoke(null, theme);
        }
    }
}