using System;
using System.Globalization;
using System.Windows.Data;

namespace MovieBase.Converters
{
    public class ZeroToNullConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int A)
            {
                if (A == 0)
                {
                    return null;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return 0;
            }
            return value;
        }
    }
}