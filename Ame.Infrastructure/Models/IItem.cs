using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public interface IItem
    {
        #region fields

        #endregion fields


        #region constructor

        #endregion constructor


        #region properties

        BindableProperty<string> Name { get; set; }
        ObservableCollection<IItem> Items { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
