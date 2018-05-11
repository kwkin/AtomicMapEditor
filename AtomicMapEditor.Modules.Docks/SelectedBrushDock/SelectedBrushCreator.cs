using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Docks.SelectedBrushDock
{
    public class SelectedBrushCreator : IDockCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public SelectedBrushCreator(IEventAggregator eventAggregator, IScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            if (scrollModel == null)
            {
                throw new ArgumentNullException("scrollModel is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
            this.Container.RegisterInstance<IScrollModel>(scrollModel);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return this.Container.Resolve<SelectedBrushViewModel>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(SelectedBrushViewModel).Equals(type);
        }

        #endregion methods
    }
}
