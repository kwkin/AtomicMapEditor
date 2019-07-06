using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Exceptions;
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

namespace Ame.App.Wpf.UI.Interactions.ProjectProperties
{
    public class EditProjectInteraction : IWindowInteraction
    {
        #region fields

        #endregion fields


        #region Constructor

        public EditProjectInteraction()
        {
        }

        public EditProjectInteraction(Project project)
            : this(project, null)
        {
        }

        public EditProjectInteraction(Action<INotification> callback)
            : this(null, callback)
        {
        }

        public EditProjectInteraction(Project project, Action<INotification> callback)
        {
            this.Project = project;
            this.Callback = callback;
        }

        #endregion Constructor


        #region Properties

        public Project Project { get; set; }
        public string Title { get; set; }
        public Action<INotification> Callback { get; set; }
        public IEventAggregator EventAggregator { get; set; }
        public double Width { get; set; } = 420.0;
        public double Height { get; set; } = 400.0;

        #endregion Properties


        #region methods

        public void UpdateMissingContent(AmeSession session)
        {
            this.Project = this.Project ?? session.CurrentProject;
            this.Title = string.Format("Edit Project - {0}", this.Project.Name);
        }

        public void RaiseNotification(DependencyObject parent)
        {
            if (this.Project == null)
            {
                throw new InteractionConfigurationException("Project is null");
            }
            Confirmation mapConfirmation = new Confirmation();
            mapConfirmation.Content = this.Project;
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
            action.WindowContent = new EditProjectWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, this.Height));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, this.Height));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
