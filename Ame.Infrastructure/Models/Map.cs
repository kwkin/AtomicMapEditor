using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Utils;

namespace Ame.Infrastructure.Models
{
    public class Map : INotifyPropertyChanged
    {
        #region fields

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion fields


        #region constructor

        public Map()
        {
            this.Name = "";
            this.Columns = 32;
            this.Rows = 32;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.LayerList.Add(new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns));
        }

        public Map(string name)
        {
            this.Name = name;
            this.Columns = 32;
            this.Rows = 32;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.LayerList.Add(new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns));
        }

        public Map(string name, int width, int height)
        {
            this.Name = name;
            this.Columns = width;
            this.Rows = height;
            this.TileWidth = 32;
            this.TileHeight = 32;
            this.Scale = ScaleType.Tile;
            this.PixelScale = 1;
            this.Description = "";
            this.LayerList = new ObservableCollection<ILayer>();
            this.TilesetList = new ObservableCollection<TilesetModel>();
            this.LayerList.Add(new Layer("Layer #0", this.TileWidth, this.TileHeight, this.Rows, this.Columns));
        }

        #endregion constructor


        #region properties
        
        private string name { get; set; }

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

        [MetadataProperty(MetadataType.Property)]
        public int Columns { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public int Rows { get; set; }

        [MetadataProperty(MetadataType.Property)]
        public ScaleType Scale { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public int TileWidth { get; set; }

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public int TileHeight { get; set; }

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

        #endregion properties


        #region methods

        public void SetWidth(int width)

        {

            this.Columns = width;

        }
        
        public void SetHeight(int height)

        {

            this.Rows = height;

        }
        
        public int GetPixelWidth()
        {
            int width = this.Columns;
            switch (this.Scale)
            {
                case ScaleType.Tile:
                    width *= this.TileWidth;
                    break;

                case ScaleType.Pixel:
                default:
                    break;
            }
            return width;
        }

        public int GetPixelHeight()
        {
            int height = this.Rows;
            switch (this.Scale)
            {
                case ScaleType.Tile:
                    height *= this.TileHeight;
                    break;
            
                case ScaleType.Pixel:
                default:
                    break;
            }
            return height;
        }

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
            int layerGroupCount = 0;
            foreach (ILayer layer in this.LayerList)
            {
                if (layer is LayerGroup)
                {
                    layerGroupCount++;
                }
            }
            return layerGroupCount;
        }

        private int GetLayerCount()
        {
            int layerCount = 0;
            foreach (ILayer layer in this.LayerList)
            {
                if (layer is Layer)
                {
                    layerCount++;
                }
            }
            return layerCount;
        }

        #endregion methods
    }
}
