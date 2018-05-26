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
            this.Name = groupName;
            this.Items = new ObservableCollection<IItem>();
        }

        public ItemGroup(string groupName, ObservableCollection<IItem> items)
        {
            this.Name = groupName;
            this.Items = items;
        }

        #endregion constructor


        #region properties

        public string Name { get; set; }
        public ObservableCollection<IItem> Items { get; set; }

        #endregion properties


        #region methods

        #endregion methods
    }
}
