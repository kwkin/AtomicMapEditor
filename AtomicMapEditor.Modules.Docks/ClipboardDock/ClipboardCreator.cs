using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Docks.ClipboardDock
{
    public class ClipboardCreator : IDockCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public ClipboardCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return this.Container.Resolve<ClipboardViewModel>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(ClipboardViewModel).Equals(type);
        }

        #endregion methods
    }
}
