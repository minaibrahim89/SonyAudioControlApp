using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace SonyAudioControl.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var boolValue = (bool)value;

            if (parameter?.ToString().ToLower() == "invert")
                boolValue ^= true;

            return boolValue ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var isVisible = (Visibility)value == Visibility.Visible;
            var invert = parameter?.ToString().ToLower() == "invert";

            return isVisible ^ invert;
        }
    }
}
