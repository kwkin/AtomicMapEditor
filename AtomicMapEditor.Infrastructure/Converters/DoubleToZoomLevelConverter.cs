using System;
using System.Globalization;
using System.Windows.Data;
using AtomicMapEditor.Infrastructure.Models;

namespace AtomicMapEditor.Infrastructure.Converters
{
    public class DoubleToZoomLevelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }
            IConvertible convert = value as IConvertible;
            return new ZoomLevel(convert.ToDouble(null));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return value;
        }
    }
}
