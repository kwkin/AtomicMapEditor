﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Prism.Mvvm;
using Ame.Infrastructure.BaseTypes;

namespace Ame.Infrastructure.Models
{
    public class Layer : ILayer, IContainsCustomProperties
    {
        #region fields
        
        #endregion fields


        #region constructor

        public Layer(Map map)
        {
            this.Map = map;

            this.Name.Value = "";
            this.TileWidth.Value = 32;
            this.TileHeight.Value = 32;
            this.Rows.Value = 32;
            this.Columns.Value = 32;
            this.IsVisible.Value = true;
            this.Position.Value = LayerPosition.Base;
            this.Scale.Value = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
            this.CustomProperties = new ObservableCollection<MetadataProperty>();

            this.Columns.PropertyChanged += LayerSizeChanged;
            this.Rows.PropertyChanged += LayerSizeChanged;
            this.OffsetX.PropertyChanged += LayerPositionChanged;
            this.OffsetY.PropertyChanged += LayerPositionChanged;
        }

        public Layer(Map map, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Map = map;

            this.Name.Value = "";
            this.TileWidth.Value = tileWidth;
            this.TileHeight.Value = tileHeight;
            this.Rows.Value = rows;
            this.Columns.Value = columns;
            this.IsVisible.Value = true;
            this.Position.Value = LayerPosition.Base;
            this.Scale.Value = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
            this.CustomProperties = new ObservableCollection<MetadataProperty>();

            this.Columns.PropertyChanged += LayerSizeChanged;
            this.Rows.PropertyChanged += LayerSizeChanged;
            this.OffsetX.PropertyChanged += LayerPositionChanged;
            this.OffsetY.PropertyChanged += LayerPositionChanged;
        }

        public Layer(Map map, string layerName, int tileWidth, int tileHeight, int rows, int columns)
        {
            this.Map = map;

            this.Name.Value = layerName;
            this.TileWidth.Value = tileWidth;
            this.TileHeight.Value = tileHeight;
            this.Rows.Value = rows;
            this.Columns.Value = columns;
            this.IsVisible.Value = true;
            this.Position.Value = LayerPosition.Base;
            this.Scale.Value = ScaleType.Tile;
            this.TileIDs = this.TileIDs ?? new TileCollection(this);
            this.CustomProperties = new ObservableCollection<MetadataProperty>();

            this.Columns.PropertyChanged += LayerSizeChanged;
            this.Rows.PropertyChanged += LayerSizeChanged;
            this.OffsetX.PropertyChanged += LayerPositionChanged;
            this.OffsetY.PropertyChanged += LayerPositionChanged;
        }

        #endregion constructor


        #region properties

        public int ID { get; set; } = -1;
        
        [MetadataProperty(MetadataType.Property, "Name")]
        public BindableProperty<string> Name { get; set; } = BindableProperty.Prepare<string>(string.Empty);

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<int> Columns { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<int> Rows { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Width")]
        public BindableProperty<int> TileWidth { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Tile Height")]
        public BindableProperty<int> TileHeight { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Pixel Offset X")]
        public BindableProperty<int> OffsetX { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property, "Pixel Offset Y")]
        public BindableProperty<int> OffsetY { get; set; } = BindableProperty.Prepare<int>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<LayerPosition> Position { get; set; } = BindableProperty.Prepare<LayerPosition>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<ScaleType> Scale { get; set; } = BindableProperty.Prepare<ScaleType>();

        [MetadataProperty(MetadataType.Property, "Scroll Rate")]
        public BindableProperty<double> ScrollRate { get; set; } = BindableProperty.Prepare<double>();

        [MetadataProperty(MetadataType.Property)]
        public BindableProperty<string> Description { get; set; } = BindableProperty.Prepare<string>();

        public BindableProperty<bool> IsImmutable { get; set; } = BindableProperty.Prepare<bool>();

        public BindableProperty<bool> IsVisible { get; set; } = BindableProperty.Prepare<bool>();

        [IgnoreNodeBuilder]
        public DrawingGroup Group
        {
            get
            {
                return this.TileIDs.Group;
            }

            set
            {
                this.TileIDs.Group = value;
            }
        }
        
        [IgnoreNodeBuilder]
        public DrawingCollection LayerItems
        {
            get
            {
                DrawingCollection children = null;
                if (this.TileIDs.Group != null)
                {
                    children = this.TileIDs.Group.Children;
                }
                return children;
            }
        }
        
        public TileCollection TileIDs { get; set; }

        public LayerGroup Parent { get; set; }

        public Map Map { get; set; }

        public ObservableCollection<MetadataProperty> CustomProperties { get; set; }

        #endregion properties


        #region methods

        public int GetPixelWidth()
        {
            return this.TileWidth.Value * this.Columns.Value;
        }

        public int GetPixelHeight()
        {
            return this.TileHeight.Value * this.Rows.Value;
        }

        public Rect GetBounds()
        {
            return new Rect(this.OffsetX.Value, this.OffsetY.Value, this.GetPixelWidth(), this.GetPixelHeight());
        }

        public Rect GetBoundsExclusive()
        {
            Rect test = new Rect(this.OffsetX.Value + 1, this.OffsetY.Value + 1, this.GetPixelWidth() - 2, this.GetPixelHeight() - 2);
            return test;
        }

        public void Clear()
        {
            this.TileIDs.Clear();
        }
        
        public Point GetPointFromIndex(int id)
        {
            int pointX = (id % this.Columns.Value) * this.TileWidth.Value;
            int pointY = (int)Math.Floor((double)(id / this.Rows.Value)) * this.TileHeight.Value;
            return new Point(pointX, pointY);
        }

        public void AddWith(ILayer layer)
        {
            if (this.Parent == null)
            {
                int thisIndex = this.Map.Layers.IndexOf(this);
                if (thisIndex != -1 && thisIndex < this.Map.LayerCount)
                {
                    this.Map.Layers.Insert(thisIndex + 1, layer);
                }
                else
                {
                    this.Map.Layers.Add(layer);
                }
            }
            else
            {
                this.Parent.AddWith(layer);
            }
        }

        private void LayerSizeChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TileIDs.Resize(0, 0);
        }

        private void LayerPositionChanged(object sender, PropertyChangedEventArgs e)
        {
            this.TileIDs.Move(this.OffsetX.Value, this.OffsetY.Value);
        }

        #endregion methods
    }
}
