using System;
using System.Windows.Input;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ToolboxDock
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

        private int brushColumnCount;
        public int BrushColumnCount
        {
            get
            {
                return this.brushColumnCount;
            }
            set
            {
                if (SetProperty(ref this.brushColumnCount, value))
                {
                    UpdateBrushModel();
                }
            }
        }

        private int brushRowCount;
        public int BrushRowCount
        {
            get
            {
                return this.brushRowCount;
            }
            set
            {
                if (SetProperty(ref this.brushRowCount, value))
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
            this.brushColumnCount = this.BrushModel.ColumnCount();
            this.brushRowCount = this.BrushModel.RowCount();
            this.brushTileHeight = this.BrushModel.TileHeight;
            this.brushTileWidth = this.BrushModel.TileWidth;
            this.brushTileOffsetX = this.BrushModel.TileOffsetX;
            this.brushTileOffsetY = this.BrushModel.TileOffsetY;

            RaisePropertyChanged(nameof(this.BrushColumnCount));
            RaisePropertyChanged(nameof(this.brushRowCount));
            RaisePropertyChanged(nameof(this.brushTileHeight));
            RaisePropertyChanged(nameof(this.brushTileWidth));
            RaisePropertyChanged(nameof(this.brushTileOffsetX));
            RaisePropertyChanged(nameof(this.brushTileOffsetY));
        }

        private void UpdateBrushModel()
        {
            this.BrushModel.SetHeightWithRows(this.BrushRowCount, this.BrushTileHeight);
            this.BrushModel.SetWidthWithColumns(this.BrushColumnCount, this.BrushTileWidth);
            this.BrushModel.TileOffsetX = this.BrushTileOffsetX;
            this.BrushModel.TileOffsetY = this.BrushTileOffsetY;
            PublishBrushModel();
        }

        private void PublishBrushModel()
        {
            this.eventAggregator.GetEvent<UpdatePaddedBrushEvent>().Publish(this.BrushModel);
        }

        #endregion methods
    }
}
