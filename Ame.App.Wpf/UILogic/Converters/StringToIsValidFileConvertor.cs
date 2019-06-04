using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Ame.App.Wpf.UILogic.Converters
{
    public class StringToIsValidFileConvertor : IValueConverter
    {
        #region fields

        #endregion fields


        #region constructor

        public StringToIsValidFileConvertor()
        {
        }

        #endregion constructor


        #region properties

        public bool IsInversed { get; set; }

        #endregion properties


        #region methods

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool isValidFile;
            if (value == null)
            {
                isValidFile = false;
            }
            else if (!typeof(string).Equals(value.GetType()))
            {
                isValidFile = false;
            }
            else
            {
                string stringValue = (string)value;
                isValidFile = File.Exists(stringValue);
            }
            return isValidFile ^ IsInversed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new NotSupportedException("Convertback unsupported");
        }

        #endregion methods
    }
}
