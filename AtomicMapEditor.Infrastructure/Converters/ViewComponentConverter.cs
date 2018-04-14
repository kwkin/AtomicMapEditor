using System;
using System.Globalization;
using System.Windows.Data;


namespace Ame.Infrastructure.Converters
{
    public class ViewComponentConverter : IValueConverter
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
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion methods
    }
}
