﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;


namespace Ame.App.Wpf.UILogic.Converters
{
    public class ViewComponentConverter : IValueConverter
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
            return parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        #endregion methods
    }
}
