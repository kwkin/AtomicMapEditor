using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace Ame.App.Wpf.UILogic.Converters
{
    public class EnumToBooleanConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object trueValue, System.Globalization.CultureInfo culture)
        {
            if (value != null && value.GetType().IsEnum)
            {
                return (Enum.Equals(value, trueValue));
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        public object ConvertBack(object value, Type targetType, object trueValue, System.Globalization.CultureInfo culture)
        {
            if (value is bool && (bool)value)
            {
                return trueValue;
            }
            else
            {
                return DependencyProperty.UnsetValue;
            }
        }

        #endregion methods
    }
}
