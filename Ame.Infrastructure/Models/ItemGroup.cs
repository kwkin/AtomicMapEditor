using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public class ItemGroup : IItem
    {
        #region fields

        #endregion fields


        #region constructor

        public ItemGroup(string groupName)
        {
            this.Name.Value = groupName;
            this.Items = new ObservableCollection<IItem>();
        }

        public ItemGroup(string groupName, ObservableCollection<IItem> items)
        {
            this.Name.Value = groupName;
            this.Items = items;
        }

        #endregion constructor


        #region properties

        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);
        public ObservableCollection<IItem> Items { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
