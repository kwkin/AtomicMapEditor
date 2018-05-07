using System;
using System.Collections.ObjectModel;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Docks.LayerListDock
{
    public class LayerListCreator : IDockCreator
    {
        #region fields

        #endregion fields


        #region constructors

        public LayerListCreator(IEventAggregator eventAggregator, ObservableCollection<ILayer> layerList)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            if (layerList == null)
            {
                throw new ArgumentNullException("layerList is null");
            }
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
            this.Container.RegisterInstance<ObservableCollection<ILayer>>(layerList);
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return this.Container.Resolve(typeof(LayerListViewModel)) as DockViewModelTemplate;
        }

        public bool AppliesTo(Type type)
        {
            return typeof(LayerListViewModel).Equals(type);
        }

        #endregion methods
    }
}
