using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Events;
using Ame.Infrastructure.Messages;
using Ame.Infrastructure.Models;
using Ame.Modules.MapEditor.Editor;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.MapEditorWindow
{
    public class NewMapInteraction : IWindowInteraction
    {
        #region fields

        private AmeSession session;
        private IEventAggregator eventAggregator;
        private InteractionRequest<INotification> interaction;

        #endregion fields


        #region Constructor

        public NewMapInteraction(AmeSession session, IEventAggregator eventAggregator)
        {
            this.session = session;
            this.eventAggregator = eventAggregator;
            this.interaction = new InteractionRequest<INotification>();
        }

        #endregion Constructor


        #region Properties

        #endregion Properties


        #region methods

        public void RaiseNotification(DependencyObject parent, Action<INotification> callback)
        {
            Confirmation mapConfirmation = new Confirmation();
            int mapCount = this.session.MapList.Count;
            string newMapName = string.Format("Map #{0}", mapCount);
            mapConfirmation.Content = new Map(newMapName);
            mapConfirmation.Title = "New Map";

            InteractionRequestTrigger trigger = new InteractionRequestTrigger();
            trigger.SourceObject = this.interaction;
            trigger.Actions.Add(GetAction());
            trigger.Attach(parent);
            this.interaction.Raise(mapConfirmation, callback);
        }

        public void OnWindowClosed(INotification notification)
        {
            IConfirmation confirmation = notification as IConfirmation;
            if (confirmation.Confirmed)
            {
                Map mapModel = confirmation.Content as Map;

                IUnityContainer container = new UnityContainer();
                container.RegisterInstance<IEventAggregator>(this.eventAggregator);
                container.RegisterInstance<IScrollModel>(new ScrollModel());
                container.RegisterInstance<Map>(mapModel);
                container.RegisterInstance(this.session);

                IList<ILayer> layerList = this.session.CurrentMap.LayerList;
                ObservableCollection<ILayer> layerObservableList = new ObservableCollection<ILayer>(layerList);
                container.RegisterInstance<ObservableCollection<ILayer>>(layerObservableList);

                this.session.MapList.Add(mapModel);
                
                OpenDockMessage openEditorMessage = new OpenDockMessage(typeof(MapEditor.Editor.MapEditorViewModel), container);
                this.eventAggregator.GetEvent<OpenDockEvent>().Publish(openEditorMessage);
            }
        }

        private PopupWindowAction GetAction()
        {
            PopupWindowAction action = new PopupWindowAction();
            action.IsModal = true;
            action.CenterOverAssociatedObject = true;
            action.WindowContent = new MapEditorWindow();

            Style style = new Style();
            style.TargetType = typeof(Window);
            style.Setters.Add(new Setter(Window.MinWidthProperty, 420.0));
            style.Setters.Add(new Setter(Window.MinHeightProperty, 380.0));
            style.Setters.Add(new Setter(Window.WidthProperty, 420.0));
            style.Setters.Add(new Setter(Window.HeightProperty, 380.0));
            action.WindowStyle = style;
            return action;
        }

        #endregion methods
    }
}
