using System;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.PreferencesWindow
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
