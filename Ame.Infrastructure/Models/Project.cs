using Ame.Infrastructure.BaseTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ame.Infrastructure.Models
{
    public class Project
    {
        #region fields

        #endregion fields


        #region constructor

        public Project(string name)
        {
            this.Name.Value = name;
            this.Maps = new ObservableCollection<Map>();
            this.Tilesets = new ObservableCollection<TilesetModel>();
        }

        #endregion constructor


        #region properties

        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>("");
        public BindableProperty<string> FileLocation { get; set; } = BindableProperty.Prepare<string>("");
        public ObservableCollection<Map> Maps { get; set; }
        public ObservableCollection<TilesetModel> Tilesets { get; set; }

        public int MapCount
        {
            get
            {
                return this.Maps.Count;
            }
        }

        public int TilesetCount
        {
            get
            {
                return this.Tilesets.Count;
            }
        }

        #endregion properties


        #region methods

        #endregion methods
    }
}
