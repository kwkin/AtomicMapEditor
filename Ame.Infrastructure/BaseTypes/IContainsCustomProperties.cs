﻿using Ame.Infrastructure.Attributes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.BaseTypes
{
    public interface IContainsCustomProperties
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        ObservableCollection<MetadataProperty> CustomProperties { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
