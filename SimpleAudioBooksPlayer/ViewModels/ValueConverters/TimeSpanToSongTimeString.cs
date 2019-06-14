using System;
using Windows.UI.Xaml.Data;
using SimpleAudioBooksPlayer.ViewModels.Extensions;

namespace SimpleAudioBooksPlayer.ViewModels.ValueConverters
{
    public class TimeSpanToSongTimeString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((TimeSpan) value).ToSongTimeString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}