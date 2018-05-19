using System;
using Ame.Infrastructure.BaseTypes;
using Microsoft.Practices.Unity;
using Prism.Events;

namespace Ame.Modules.Windows.Docks.ItemListDock
{
    public class ItemListCreator : IDockCreator
    {
        #region fields

        private IEventAggregator eventAggregator;

        #endregion fields


        #region constructors

        public ItemListCreator(IEventAggregator eventAggregator)
        {
            if (eventAggregator == null)
            {
                throw new ArgumentNullException("eventAggregator is null");
            }
            this.eventAggregator = eventAggregator;
        }

        #endregion constructors


        #region properties

        #endregion properties


        #region methods

        public DockViewModelTemplate CreateDock()
        {
            return new ItemListViewModel(this.eventAggregator);
        }

        public bool AppliesTo(Type type)
        {
            return typeof(ItemListViewModel).Equals(type);
        }

        public void UpdateContent(Type type, object value)
        {
            if (typeof(IEventAggregator).Equals(type))
            {
                this.eventAggregator = value as IEventAggregator;
            }
        }

        #endregion methods
    }
}
