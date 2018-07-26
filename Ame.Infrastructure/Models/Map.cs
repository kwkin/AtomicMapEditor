using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;
using Ame.Infrastructure.Attributes;
using Ame.Infrastructure.Models.DrawingBrushes;

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

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.RowCount, this.ColumnCount);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<BrushAction>();
            this.RedoQueue = new Stack<BrushAction>();
            for (int xIndex = 0; xIndex < this.TileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.TileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Rect rect = new Rect(position, new Size(32, 32));
                    ImageDrawing emptyTile = new ImageDrawing(new DrawingImage(), rect);
                    this.CurrentLayer.LayerItems.Add(emptyTile);
                }
            }
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

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.RowCount, this.ColumnCount);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<BrushAction>();
            this.RedoQueue = new Stack<BrushAction>();
            for (int xIndex = 0; xIndex < this.TileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.TileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Rect rect = new Rect(position, new Size(32, 32));
                    ImageDrawing emptyTile = new ImageDrawing(new DrawingImage(), rect);
                    this.CurrentLayer.LayerItems.Add(emptyTile);
                }
            }
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

            Layer initialLayer = new Layer("Layer #0", this.TileWidth, this.TileHeight, this.RowCount, this.ColumnCount);
            this.LayerList.Add(initialLayer);

            this.UndoQueue = new Stack<BrushAction>();
            this.RedoQueue = new Stack<BrushAction>();
            for (int xIndex = 0; xIndex < this.TileWidth; ++xIndex)
            {
                for (int yIndex = 0; yIndex < this.TileHeight; ++yIndex)
                {
                    Point position = new Point(xIndex * 32, yIndex * 32);
                    Rect rect = new Rect(position, new Size(32, 32));
                    ImageDrawing emptyTile = new ImageDrawing(new DrawingImage(), rect);
                    this.CurrentLayer.LayerItems.Add(emptyTile);
                }
            }
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
        public int ColumnCount
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
        public int RowCount
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

        public Layer CurrentLayer
        {
            get
            {
                return this.LayerList[this.SelectedLayerIndex] as Layer;
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

        public Stack<BrushAction> UndoQueue { get; set; }
        public Stack<BrushAction> RedoQueue { get; set; }

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

        public void Draw(BrushAction action)
        {
            BrushAction undoAction = applyAction(action);
            this.UndoQueue.Push(undoAction);
            this.RedoQueue.Clear();
        }

        public void Undo()
        {
            if (this.UndoQueue.Count == 0)
            {
                return;
            }
            BrushAction undoAction = this.UndoQueue.Pop();
            BrushAction redoAction = applyAction(undoAction);
            this.RedoQueue.Push(redoAction);
        }

        public void Redo()
        {
            if (this.RedoQueue.Count == 0)
            {
                return;
            }
            BrushAction redoAction = this.RedoQueue.Pop();
            BrushAction undoAction = applyAction(redoAction);
            this.UndoQueue.Push(undoAction);
        }

        private BrushAction applyAction(BrushAction action)
        {
            Stack<ImageDrawing> previousTiles = new Stack<ImageDrawing>();
            foreach (ImageDrawing tile in action.Tiles)
            {
                ImageDrawing previousTile = Draw(tile);
                if (previousTile != null)
                {
                    previousTiles.Push(previousTile);
                }
            }
            BrushAction revertAction = new BrushAction(action.Name, previousTiles);
            return revertAction;
        }
        
        private ImageDrawing Draw(ImageDrawing tile)
        {
            if (tile.Bounds.X < 0
                || tile.Bounds.Y < 0 
                || tile.Bounds.X >= this.PixelWidth 
                || tile.Bounds.Y >= this.PixelHeight)
            {
                return null;
            }
            int previousTileIndex = (int)(tile.Bounds.X / this.TileWidth) + (int)(tile.Bounds.Y / this.TileHeight) * this.ColumnCount;
            ImageDrawing previousTile = this.CurrentLayer.LayerItems[previousTileIndex] as ImageDrawing;
            this.CurrentLayer.LayerItems[previousTileIndex] = tile;
            previousTile.Rect = tile.Bounds;
            return previousTile;
        }

        #endregion methods
    }
}
