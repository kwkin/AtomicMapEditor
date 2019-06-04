using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ame.App.Wpf.UILogic.Converters
{
    public class NullOrEmptyToBooleanConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        public NullOrEmptyToBooleanConverter()
        {
        }

        #endregion constructor


        #region properties

        public bool IsInversed { get; set; }

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isNullOrEmpty;
            if (value == null)
            {
                isNullOrEmpty = true;
            }
            else if (!typeof(string).Equals(value.GetType()))
            {
                isNullOrEmpty = true;
            }
            else
            {
                string stringValue = (string)value;
                isNullOrEmpty = string.IsNullOrEmpty(stringValue) ? true : false;
            }
            return isNullOrEmpty ^ IsInversed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException("Convertback unsupported");
        }

        #endregion methods
    }
}
