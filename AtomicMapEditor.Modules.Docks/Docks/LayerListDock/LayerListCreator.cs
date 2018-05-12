using System;
using System.Collections.ObjectModel;
using Ame.Infrastructure.BaseTypes;
using Ame.Infrastructure.Models;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.LayerListDock
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
            this.Container = new UnityContainer();
            this.Container.RegisterInstance<IEventAggregator>(eventAggregator);
            if (layerList != null)
            {
                this.Container.RegisterInstance<ObservableCollection<ILayer>>(layerList);
            }
            else
            {
                this.Container.RegisterInstance<ObservableCollection<ILayer>>(new ObservableCollection<ILayer>());
            }
        }

        #endregion constructors


        #region properties

        public IUnityContainer Container { get; set; }

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return this.Container.Resolve<LayerListViewModel>();
        }

        public bool AppliesTo(Type type)
        {
            return typeof(LayerListViewModel).Equals(type);
        }

        #endregion methods
    }
}
