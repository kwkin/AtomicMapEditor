using System;
using Ame.Components.Behaviors;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Docks.ItemEditorDock
{
    public class ItemEditorCreator : IDockCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public ItemEditorCreator(IEventAggregator eventAggregator, ScrollModel scrollModel)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
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
            return this.Container.Resolve<ItemEditorViewModel>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(ItemEditorViewModel).Equals(type);
        }

        #endregion methods
    }
}
