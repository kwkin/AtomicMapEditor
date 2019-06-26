using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.DrawingTools;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            this.eventAggregator = eventAggregator ?? throw new ArgumentNullException("eventAggregator");
            this.session = session ?? throw new ArgumentNullException("session");

            this.Title.Value = "Tool";

            this.DrawingTool.PropertyChanged += DrawingToolChanged;
            this.IsEraser.PropertyChanged += IsEraserChanged;
            this.BrushColumns.PropertyChanged += BrushColumnsChanged;
            this.BrushRows.PropertyChanged += BrushRowsChanged;
            this.BrushTileWidth.PropertyChanged += BrushTileWidthChanged;
            this.BrushTileHeight.PropertyChanged += BrushTileHeightChanged;
            this.BrushTileOffsetX.PropertyChanged += BrushTileOffsetXChanged;
            this.BrushTileOffsetY.PropertyChanged += BrushTileOffsetYChanged;

            this.BrushModel.PropertyChanged += BrushModelChanged;

            this.ToolButtonCommand = new DelegateCommand<Type>((brush) =>
            {
                this.DrawingTool.Value = Activator.CreateInstance(brush) as IDrawingTool;
            });

            this.eventAggregator.GetEvent<NewPaddedBrushEvent>().Subscribe((brushModel) =>
            {
                this.BrushModel.Value = brushModel;
            }, ThreadOption.PublisherThread);
        }

        #endregion constructor


        #region properties

        public ICommand ToolButtonCommand { get; private set; }

        public BindableProperty<IDrawingTool> DrawingTool { get; set; } = BindableProperty<IDrawingTool>.Prepare();

        public BindableProperty<bool> IsEraser { get; set; } = BindableProperty<bool>.Prepare();

        public BindableProperty<int> BrushColumns { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> BrushRows { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> BrushTileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> BrushTileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> BrushTileOffsetX { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> BrushTileOffsetY { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> MaxTileWidth { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> MaxTileHeight { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> MaxTileOffsetX { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<int> MaxTileOffsetY { get; set; } = BindableProperty<int>.Prepare();

        public BindableProperty<PaddedBrushModel> BrushModel { get; set; } = BindableProperty<PaddedBrushModel>.Prepare();

        #endregion properties


        #region methods

        public override void CloseDock()
        {
            CloseDockMessage closeMessage = new CloseDockMessage(this);
            this.eventAggregator.GetEvent<CloseDockEvent>().Publish(closeMessage);
        }

        public void SetToolboxTitle()
        {
            if (!string.IsNullOrEmpty(this.DrawingTool.Value.ToolName))
            {
                this.Title.Value = "Tool - " + this.DrawingTool.Value.ToolName;
            }
            else
            {
                this.Title.Value = "Tool";
            }
        }

        private void BrushModelChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateBrushProperties(this.BrushModel.Value);
        }

        private void IsEraserChanged(object sender, PropertyChangedEventArgs e)
        {
            if (typeof(IEraserTool).IsAssignableFrom(this.DrawingTool.GetType()))
            {
                IEraserTool erasorTool = this.session.DrawingTool as IEraserTool;
                erasorTool.IsErasing = this.IsEraser.Value;
            }
        }

        private void DrawingToolChanged(object sender, PropertyChangedEventArgs e)
        {
            SetToolboxTitle();
        }

        private void BrushTileOffsetYChanged(object sender, PropertyChangedEventArgs e)
        {
            this.BrushModel.Value.TileOffsetY.Value = this.BrushTileOffsetY.Value;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void BrushTileOffsetXChanged(object sender, PropertyChangedEventArgs e)
        {
            this.BrushModel.Value.TileOffsetX.Value = this.BrushTileOffsetX.Value;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void BrushTileHeightChanged(object sender, PropertyChangedEventArgs e)
        {
            this.BrushModel.Value.TileHeight.Value = this.BrushTileHeight.Value;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void BrushTileWidthChanged(object sender, PropertyChangedEventArgs e)
        {
            this.BrushModel.Value.TileWidth.Value = this.BrushTileWidth.Value;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void BrushRowsChanged(object sender, PropertyChangedEventArgs e)
        {
            this.BrushModel.Value.Rows.Value = this.BrushRows.Value;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void BrushColumnsChanged(object sender, PropertyChangedEventArgs e)
        {
            this.BrushModel.Value.Columns.Value = this.BrushColumns.Value;
            UpdateStampLimits();
            PublishBrushModel();
        }

        private void UpdateBrushProperties(PaddedBrushModel brushModel)
        {
            PaddedBrushModel brush = this.BrushModel.Value;
            this.BrushColumns.Value = brush.Columns.Value;
            this.BrushRows.Value = brush.Rows.Value;
            this.BrushTileHeight.Value = brush.TileHeight.Value;
            this.BrushTileWidth.Value = brush.TileWidth.Value;
            this.BrushTileOffsetX.Value = brush.TileOffsetX.Value;
            this.BrushTileOffsetY.Value = brush.TileOffsetY.Value;
            UpdateStampLimits();
        }

        private void PublishBrushModel()
        {
            this.eventAggregator.GetEvent<UpdatePaddedBrushEvent>().Publish(this.BrushModel.Value);
        }

        private void UpdateStampLimits()
        {
            if (this.session.CurrentTileset != null)
            {
                this.MaxTileWidth.Value = this.session.CurrentTileset.Columns.Value - this.BrushTileOffsetX.Value;
                this.MaxTileHeight.Value = this.session.CurrentTileset.Rows.Value - this.BrushTileOffsetY.Value;
                this.MaxTileOffsetX.Value = this.session.CurrentTileset.Columns.Value - this.BrushColumns.Value;
                this.MaxTileOffsetY.Value = this.session.CurrentTileset.Rows.Value - this.BrushRows.Value;
            }
        }

        #endregion methods
    }
}
