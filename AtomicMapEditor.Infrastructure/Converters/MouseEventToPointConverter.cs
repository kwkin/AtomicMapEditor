using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ame.Infrastructure.Converters
{
    public class MouseEventToPointConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor & destructer

        #endregion constructor & destructer


        #region properties

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = (MouseEventArgs)value;
            var element = (FrameworkElement)parameter;

            var point = args.GetPosition(element);
            return point;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion methods
    }
}
