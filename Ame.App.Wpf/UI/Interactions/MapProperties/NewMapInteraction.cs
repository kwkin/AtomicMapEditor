using Ame.App.Wpf.UI.Editor.MapEditor;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Events.Messages;
using Ame.Infrastructure.Models;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ame.App.Wpf.UI.Interactions.MapProperties
{
    public class NewMapInteraction : IWindowInteraction
    {
        #region fields

        private Map map;
        private IAmeSession session;

        #endregion fields


        #region Constructor

        public NewMapInteraction()
        {
        }

        public NewMapInteraction(Project project)
            :this (project, null)
        {
        }

        public NewMapInteraction(Action<INotification> callback)
            : this(null, callback)
        {
            this.Callback = callback;
        }

        public NewMapInteraction(Project project, Action<INotification> callback)
        {
            this.Project = project;
            this.Callback = callback;
        }


        #endregion Constructor


        #region Properties

        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        public double Width { get; set; } = 420.0;
        public double Height { get; set; } = 400.0;
        public Project Project { get; set; }

        #endregion Properties


        #region methods

        public void UpdateMissingContent(IAmeSession session)
        {
            this.session = session;
            this.Project = this.Project ?? session.CurrentProject.Value;
            this.Title = "New Map";
            string newMapName = string.Format("Map #{0}", session.MapCount);
            this.map = new Map(newMapName);
            this.Callback = this.Callback ?? OnNewMapWindowClosed;
        }

        public void RaiseNotification(DependencyObject parent)
        {
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = this.map;
            mapConfirmation.Title = this.Title;

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            InteractionRequest<INotification> interaction = new InteractionRequest<INotification>();
            trigger.SourceObject = interaction;
            trigger.Actions.Add(CreateAction());
            trigger.Attach(parent);
            interaction.Raise(mapConfirmation, this.Callback);
        }

        private PopupWindowAction CreateAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new NewMapWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, this.Height));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, this.Height));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewMapWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Map newMap = confirmation.Content as Map;
                this.session.Maps.Add(newMap);
                if (this.Project != null)
                {
                    newMap.Project.Value = this.Project;
                    this.Project.Maps.Add(newMap);
                }
            }
        }

        #endregion methods
    }
}
