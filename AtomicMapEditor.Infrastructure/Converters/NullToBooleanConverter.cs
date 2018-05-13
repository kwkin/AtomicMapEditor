using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ame.Infrastructure.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        public NullToBooleanConverter()
        {
        }

        #endregion constructor


        #region properties
        
        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value == null ? false : true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return new NotSupportedException("Convertback unsupported");
        }

        #endregion methods
    }
}
