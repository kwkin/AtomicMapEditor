using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Docks.ToolboxDock
{
    public class ToolboxCreator : IDockCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public ToolboxCreator(IEventAggregator eventAggregator)
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
            return this.Container.Resolve<ToolboxViewModel>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(ToolboxViewModel).Equals(type);
        }

        #endregion methods
    }
}
