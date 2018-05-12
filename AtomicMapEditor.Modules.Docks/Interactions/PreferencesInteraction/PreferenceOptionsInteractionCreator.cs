﻿using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;

namespace Ame.Modules.Windows.Interactions.PreferencesInteraction
{
    public class PreferenceOptionsInteractionCreator : IWindowInteractionCreator
    {
        #region fields

        #endregion fields


        #region Constructor

        public PreferenceOptionsInteractionCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        public PreferenceOptionsInteractionCreator(IEventAggregator eventAggregator, Action<INotification> callback)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
            this.Container.RegisterInstance<Action<INotification>>(callback);
        }

        #endregion Constructor


        #region Properties

        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return this.Container.Resolve<PreferenceOptionsInteraction>();
        }

        public IWindowInteraction CreateWindowInteraction(Action<INotification> callback)
        {
            IUnityContainer container = new UnityContainer();
            foreach (ContainerRegistration registration in this.Container.Registrations)
            {
                container.RegisterInstance<ContainerRegistration>(registration);
            }
            container.RegisterInstance<Action<INotification>>(callback);
            return container.Resolve<PreferenceOptionsInteraction>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(PreferenceOptionsInteraction).Equals(type);
        }

        #endregion methods
    }
}
