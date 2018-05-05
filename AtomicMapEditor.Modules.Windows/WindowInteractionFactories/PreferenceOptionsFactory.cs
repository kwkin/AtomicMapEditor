﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ame.Modules.Windows.WindowInteractions;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.WindowInteractionFactories
{
    public class PreferenceOptionsFactory : IWindowInteractionFactory
    {
        #region fields

        #endregion fields


        #region Constructor

        public PreferenceOptionsFactory(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        #endregion Constructor


        #region Properties

        public IUnityContainer Container { get; set; }

        #endregion Properties


        #region methods

        public IWindowInteraction CreateWindowInteraction()
        {
            return this.Container.Resolve(typeof(PreferenceOptionsInteraction)) as IWindowInteraction;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(PreferenceOptionsInteraction).Equals(type);
        }

        #endregion methods
    }
}
