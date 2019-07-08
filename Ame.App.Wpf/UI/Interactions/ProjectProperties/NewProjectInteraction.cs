using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Infrastructure.Models.Serializer.Json;
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
    public class NewProjectInteraction : IWindowInteraction
    {
        #region fields

        private AmeSession session;

        #endregion fields


        #region Constructor

        public NewProjectInteraction()
        {
        }

        public NewProjectInteraction(Action<INotification> callback)
        {
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

        public void UpdateMissingContent(AmeSession session)
        {
            this.session = session;
            this.Title = "New Project";
            string newProjectName = string.Format("Project #{0}", session.ProjectCount);
            this.Project = new Project(newProjectName);
            this.Callback = this.Callback ?? OnNewProjectWindowClosed;
        }

        public void RaiseNotification(DependencyObject parent)
        {
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
            action.WindowContent = new NewProjectWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(FrameworkElement.MinWidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.MinHeightProperty, this.Height));
            style.Setters.Add(new Setter(FrameworkElement.WidthProperty, this.Width));
            style.Setters.Add(new Setter(FrameworkElement.HeightProperty, this.Height));
            action.WindowStyle = style;
            return action;
        }

        private void OnNewProjectWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Project newProject = confirmation.Content as Project;
                this.session.Projects.Add(newProject);
                newProject.UpdateFile();
            }
        }

        #endregion methods
    }
}
