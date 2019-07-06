using Ame.Infrastructure.Attributes;
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
            this.CustomProperties = new ObservableCollection<MetadataProperty>();
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property, "Name")]
        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Source Path")]
        public BindableProperty<string> SourcePath { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Pixel Scale")]
        public BindableProperty<int> DefaultPixelScale { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public BindableProperty<int> DefaultTileWidth { get; set; } = BindableProperty.Prepare<int>(32);

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public BindableProperty<int> DefaultTileHeight { get; set; } = BindableProperty.Prepare<int>(32);

        [MetadataProperty(MetadataType.Property, "Description")]
        public BindableProperty<string> Description { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property, "Version")]
        public BindableProperty<string> Version { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        public ObservableCollection<Map> Maps { get; set; }

        public ObservableCollection<TilesetModel> Tilesets { get; set; }

        public ObservableCollection<MetadataProperty> CustomProperties { get; set; }

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
