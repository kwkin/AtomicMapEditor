using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace Ame.Infrastructure.Converters
{
    public class MouseEventToPointConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var args = value as MouseEventArgs;
            var element = parameter as FrameworkElement;

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
