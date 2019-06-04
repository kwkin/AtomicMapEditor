using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ame.App.Wpf.UI.Docks.ToolboxDock
{
    public class ToolboxViewModel : DockToolViewModelTemplate
    {
        #region fields

        private IEventAggregator eventAggregator;
        private AmeSession session;

        #endregion fields


        #region constructor

        public ToolboxViewModel(IEventAggregator eventAggregator, AmeSession session)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            if (session == null)
            {
                throw new ArgumentNullException("session");
            }
            this.eventAggregator = eventAggregator;
            this.session = session;

            this.Title = "Tool";

            this.ToolButtonCommand = new DelegateCommand<Type>((brush) =>
            {
                this.DrawingTool = Activator.CreateInstance(brush) as IDrawingTool;
            });

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Subscribe((brushModel) =>
            {
                this.BrushModel = brushModel;
            }, ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        public ICommand ToolButtonCommand { get; private set; }

        private IDrawingTool drawingTool;
        public IDrawingTool DrawingTool
        {
            get
            {
                return this.drawingTool;
            }
            set
            {
                if (SetProperty(ref this.drawingTool, value))
                {
                    SetToolboxTitle();
                }
            }
        }

        private bool isEraser;
        public bool IsEraser
        {
            get
            {
                return this.isEraser;
            }
            set
            {
                if (SetProperty(ref this.isEraser, value))
                {
                    if (typeof(IEraserTool).IsAssignableFrom(this.DrawingTool.GetType()))
                    {
                        IEraserTool erasorTool = this.DrawingTool as IEraserTool;
                        erasorTool.IsErasing = value;
                        this.session.DrawingTool = this.DrawingTool;
                    }
                }
            }
        }

        private int brushColumns;
        public int BrushColumns
        {
            get
            {
                return this.brushColumns;
            }
            set
            {
                if (SetProperty(ref this.brushColumns, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int brushRows;
        public int BrushRows
        {
            get
            {
                return this.brushRows;
            }
            set
            {
                if (SetProperty(ref this.brushRows, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int brushTileWidth;
        public int BrushTileWidth
        {
            get
            {
                return this.brushTileWidth;
            }
            set
            {
                if (SetProperty(ref this.brushTileWidth, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int brushTileHeight;
        public int BrushTileHeight
        {
            get
            {
                return this.brushTileHeight;
            }
            set
            {
                if (SetProperty(ref this.brushTileHeight, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int brushTileOffsetX;
        public int BrushTileOffsetX
        {
            get
            {
                return this.brushTileOffsetX;
            }
            set
            {
                if (SetProperty(ref this.brushTileOffsetX, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int brushTileOffsetY;
        public int BrushTileOffsetY
        {
            get
            {
                return this.brushTileOffsetY;
            }
            set
            {
                if (SetProperty(ref this.brushTileOffsetY, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int maxTileWidth;
        public int MaxTileWidth
        {
            get
            {
                return this.maxTileWidth;
            }
            set
            {
                SetProperty(ref this.maxTileWidth, value);
            }
        }

        private int maxTilHeight;
        public int MaxTileHeight
        {
            get
            {
                return this.maxTilHeight;
            }
            set
            {
                SetProperty(ref this.maxTilHeight, value);
            }
        }

        private int maxTileOffsetX;
        public int MaxTileOffsetX
        {
            get
            {
                return this.maxTileOffsetX;
            }
            set
            {
                SetProperty(ref this.maxTileOffsetX, value);
            }
        }

        private int maxTileOffsetY;
        public int MaxTileOffsetY
        {
            get
            {
                return this.maxTileOffsetY;
            }
            set
            {
                SetProperty(ref this.maxTileOffsetY, value);
            }
        }

        private PaddedBrushModel brushModel;
        public PaddedBrushModel BrushModel
        {
            get
            {
                return this.brushModel;
            }
            set
            {
                if (SetProperty(ref this.brushModel, value))
                {
                    UpdateBrushProperties(this.brushModel);
                }
            }
        }

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void SetToolboxTitle()
        {
            if (!string.IsNullOrEmpty(this.DrawingTool.ToolName))
            {
                this.Title = "Tool - " + this.DrawingTool.ToolName;
            }
            else
            {
                this.Title = "Tool";
            }
        }

        private void UpdateBrushProperties(PaddedBrushModel brushModel)
        {
            this.brushColumns = this.BrushModel.Columns();
            this.brushRows = this.BrushModel.Rows();
            this.brushTileHeight = this.BrushModel.TileHeight;
            this.brushTileWidth = this.BrushModel.TileWidth;
            this.brushTileOffsetX = this.BrushModel.TileOffsetX;
            this.brushTileOffsetY = this.BrushModel.TileOffsetY;

            RaisePropertyChanged(nameof(this.BrushColumns));
            RaisePropertyChanged(nameof(this.brushRows));
            RaisePropertyChanged(nameof(this.brushTileHeight));
            RaisePropertyChanged(nameof(this.brushTileWidth));
            RaisePropertyChanged(nameof(this.brushTileOffsetX));
            RaisePropertyChanged(nameof(this.brushTileOffsetY));
            UpdateStampLimits();
        }

        private void UpdateBrushModel()
        {
            this.BrushModel.SetHeightWithRows(this.BrushRows, this.BrushTileHeight);
            this.BrushModel.SetWidthWithColumns(this.BrushColumns, this.BrushTileWidth);
            this.BrushModel.TileOffsetX = this.BrushTileOffsetX;
            this.BrushModel.TileOffsetY = this.BrushTileOffsetY;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void PublishBrushModel()
        {
            this.eventAggregator.GetEvent<UpdatePaddedBrushEvent>().Publish(this.BrushModel);
        }

        private void UpdateStampLimits()
        {
            if (this.session.CurrentTileset != null)
            {
                this.MaxTileWidth = this.session.CurrentTileset.Columns() - this.BrushTileOffsetX;
                this.MaxTileHeight = this.session.CurrentTileset.Rows() - this.BrushTileOffsetY;
                this.MaxTileOffsetX = this.session.CurrentTileset.Columns() - this.BrushColumns;
                this.MaxTileOffsetY = this.session.CurrentTileset.Rows() - this.BrushRows;
            }
        }

        #endregion methods
    }
}
