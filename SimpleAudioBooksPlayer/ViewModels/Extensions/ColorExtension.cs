using Windows.UI;

namespace SimpleAudioBooksPlayer.ViewModels.Extensions
{
    public static class ColorExtension
    {
        public static string ToArgbString(this Color color)
        {
            return $"{color.A},{color.R},{color.G},{color.B}";
        }
    }
}