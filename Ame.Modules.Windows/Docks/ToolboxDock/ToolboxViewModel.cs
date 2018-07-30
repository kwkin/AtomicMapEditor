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

        #endregion methods
    }
}
