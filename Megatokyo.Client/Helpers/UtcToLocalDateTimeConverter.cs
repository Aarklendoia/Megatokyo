using System;
using Windows.UI.Xaml.Data;

namespace Megatokyo.Client.Helpers
{
    internal class UtcToLocalDateTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is DateTime dateTime)
                return dateTime.ToLocalTime().ToString();
            if (value is DateTimeOffset dateTimeOffset)
                return dateTimeOffset.DateTime.ToLocalTime().ToString();
            else
                return DateTime.Parse(value?.ToString()).ToLocalTime().ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
