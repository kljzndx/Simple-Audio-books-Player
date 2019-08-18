using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SimpleAudioBooksPlayer.ViewModels.ValueConverters
{
    public class BoolToVisibilityReverse : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            bool theValue = (bool) value;
            return theValue ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var theValue = (Visibility) value;
            return theValue == Visibility.Collapsed;
        }
    }
}