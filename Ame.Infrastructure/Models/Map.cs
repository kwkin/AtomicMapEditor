using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using Ame.Infrastructure.Attributes;

namespace Ame.Infrastructure.Models
{
    public class Map : INotifyPropertyChanged
    {
        #region fields

        public event PropertyChangedEventHandler PropertyChanged;
        private string name;

        #endregion fields


        #region constructor

        public Map()
        {
            this.Name = "";
            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);
        }

        public Map(string name)
        {
            this.Name = name;

            this.Grid = new GridModel(32, 32, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);
        }

        public Map(string name, int width, int height)
        {
            this.Name = name;
            this.Grid = new GridModel(width, height, 32, 32);
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns);
            this.LayerList.Add(initialLayer);
        }

        #endregion constructor


        #region properties

        [MetadataProperty(MetadataType.Property)]
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (name != value)
                {
                    name = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public GridModel Grid { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int Columns
        {
            get
            {
                return this.Grid.ColumnCount();
            }
            set
            {
                this.Grid.SetWidthWithColumns(value);
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public int Rows
        {
            get
            {
                return this.Grid.RowCount();
            }
            set
            {
                this.Grid.SetHeightWithRows(value);
            }
        }

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth
        {
            get
            {
                return this.Grid.TileWidth;
            }
            set
            {
                this.Grid.TileWidth = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight
        {
            get
            {
                return this.Grid.TileHeight;
            }
            set
            {
                this.Grid.TileHeight = value;
            }
        }

        [MetadataProperty(MetadataType.Property)]
        public ScaleType Scale
        {
            get
            {
                return this.Grid.Scale;
            }
            set
            {
                this.Grid.Scale = value;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Width")]
        public int PixelWidth
        {
            get
            {
                return this.Grid.PixelWidth;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Height")]
        public int PixelHeight
        {
            get
            {
                return this.Grid.PixelHeight;
            }
        }

        [MetadataProperty(MetadataType.Property, "Pixel Ratio")]
        public int PixelRatio { get; set; }

        [MetadataProperty(MetadataType.Property, "Pixel Scale")]
        public int PixelScale { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public string Description { get; set; }

        public ObservableCollection<ILayer> LayerList { get; set; }
        public int SelectedLayerIndex { get; set; }

        public ILayer CurrentLayer
        {
            get
            {
                return this.LayerList[this.SelectedLayerIndex];
            }
        }

        public int LayerCount
        {
            get
            {
                return this.LayerList.Count;
            }
        }

        public ObservableCollection<TilesetModel> TilesetList { get; set; }

        public int TilesetCount
        {
            get
            {
                return this.TilesetList.Count;
            }
        }

        #endregion properties


        #region methods

        public void MergeCurrentLayerDown()
        {
            Console.WriteLine("Merge Current Layer Down");
        }

        public void MergeCurrentLayerUp()
        {
            Console.WriteLine("Merge Current Layer Up");
        }

        public void MergeVisibleLayers()
        {
            Console.WriteLine("Merge Visible Layers");
        }

        public void DeleteCurrentLayer()
        {
            this.LayerList.RemoveAt(this.SelectedLayerIndex);
        }

        public void NewLayerGroup()
        {
            int layerGroupCount = GetLayerGroupCount();
            string newLayerGroupName = string.Format("Layer Group #{0}", layerGroupCount);
            ILayer newLayerGroup = new LayerGroup(newLayerGroupName);
            this.LayerList.Add(newLayerGroup);
        }

        public void DuplicateCurrentLayer()
        {
            ILayer copiedLayer = Utils.Utils.DeepClone<ILayer>(this.CurrentLayer);
            this.LayerList.Add(copiedLayer);
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private int GetLayerGroupCount()
        {
            IEnumerable<LayerGroup> groups = this.LayerList.OfType<LayerGroup>();
            return groups.Count<LayerGroup>();
        }

        private int GetLayerCount()
        {
            IEnumerable<Layer> groups = this.LayerList.OfType<Layer>();
            return groups.Count<Layer>();
        }

        #endregion methods
    }
}
